using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SeanLib.Core
{
    public abstract class MaterialPropertyAsset : ScriptableObject, IMaterialProperty
    {
        public abstract void ApplyMaterialProperty(MaterialPropertyBlock block);

        public abstract void ApplyMaterialPropertys(int count, MaterialPropertyBlock block);
    }
    public abstract class MaterialProperty : IMaterialProperty
    {
        public abstract void ApplyMaterialProperty(MaterialPropertyBlock block);

        public abstract void ApplyMaterialPropertys(int count, MaterialPropertyBlock block);
    }

    public interface IMaterialProperty
    {
        void ApplyMaterialProperty(MaterialPropertyBlock block);
        void ApplyMaterialPropertys(int count, MaterialPropertyBlock block);
    }
}