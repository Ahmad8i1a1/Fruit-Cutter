using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;

namespace Assets.Scripts
{
    class Slicer
    {
        /// <summary>
        /// Slice the object by the plane 
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="objectToCut"></param>
        /// <returns></returns>
        public static GameObject[] Slice(Plane plane, GameObject objectToCut)

        {
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null)
            {
                throw new NotSupportedException("Cannot slice non-sliceable object");
            }

            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid, sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

            GameObject positiveObject = CreateMeshGameObject(objectToCut);
            positiveObject.name = $"{objectToCut.name}_positive";
            positiveObject.layer = LayerMask.NameToLayer("SlicedObject");

            GameObject negativeObject = CreateMeshGameObject(objectToCut);
            negativeObject.name = $"{objectToCut.name}_negative";
            negativeObject.layer = LayerMask.NameToLayer("SlicedObject");

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;
            //var positiveSideMeshData = SimplifyMesh(positiveSideMeshData, 0.3f); // Adjust quality
            //var negativeSideMeshData = SimplifyMesh(negativeSideMeshData, 0.3f);

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            SetupCollidersAndRigidBodys(ref positiveObject, positiveSideMeshData, sliceable.UseGravity);
            SetupCollidersAndRigidBodys(ref negativeObject, negativeSideMeshData, sliceable.UseGravity);

            // 1. Auto destroy after 5 seconds
            UnityEngine.Object.Destroy(positiveObject, 7f);
            UnityEngine.Object.Destroy(negativeObject, 7f);

            // 2. Disable physics on small debris after 3 seconds
            float positiveSize = positiveSideMeshData.bounds.size.magnitude;
            float negativeSize = negativeSideMeshData.bounds.size.magnitude;

            if (positiveSize < 0.5f)
            {
                positiveObject.AddComponent<DisablePhysicsAfterTime>().Setup(5f); // Faster disable
            }

            if (negativeSize < 0.5f)
            {
                negativeObject.AddComponent<DisablePhysicsAfterTime>().Setup(5f); // Faster disable
            }


            return new GameObject[] { positiveObject, negativeObject };
        }
        /// <summary>
        /// Creates the default mesh game object.
        /// </summary>
        /// <param name="originalObject">The original object.</param>
        /// <returns></returns>
        /// 


        private static GameObject CreateMeshGameObject(GameObject originalObject)
        {
            var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;

            GameObject meshGameObject = new GameObject();
            Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

            meshGameObject.AddComponent<MeshFilter>();
            meshGameObject.AddComponent<MeshRenderer>();
            Sliceable sliceable = meshGameObject.AddComponent<Sliceable>();

            sliceable.IsSolid = originalSliceable.IsSolid;
            sliceable.ReverseWireTriangles = originalSliceable.ReverseWireTriangles;
            sliceable.UseGravity = originalSliceable.UseGravity;

            meshGameObject.GetComponent<MeshRenderer>().materials = originalMaterial;

            meshGameObject.transform.localScale = originalObject.transform.localScale;
            meshGameObject.transform.rotation = originalObject.transform.rotation;
            meshGameObject.transform.position = originalObject.transform.position;

            meshGameObject.tag = originalObject.tag;

            return meshGameObject;
        }

        /// <summary>
        /// Add mesh collider and rigid body to game object
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="mesh"></param>
        private static void SetupCollidersAndRigidBodys(ref GameObject gameObject, Mesh mesh, bool useGravity)
        {
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;

            var rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = useGravity;
        }

    }
}
