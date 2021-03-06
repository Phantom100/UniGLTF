﻿using System;


namespace UniGLTF
{
    [Serializable]
    public class glTFTextureInfo: IJsonSerializable
    {
        public int index = -1;
        public int texCoord;
        //public float strength;

        // empty schemas
        public object extensions;
        public object extras;

        public string ToJson()
        {
            var f = new JsonFormatter();
            f.BeginMap();
            f.Key("index"); f.Value(index);
            f.Key("texCoord"); f.Value(texCoord);
            //f.Key("scale"); f.Value(scale);
            //f.Key("strength"); f.Value(strength);
            f.EndMap();
            return f.ToString();
        }
    }


    [Serializable]
    public class glTFMaterialNormalTextureInfo: glTFTextureInfo
    {
        public float scale;
    }

    [Serializable]
    public class glTFMaterialOcclusionTextureInfo : glTFTextureInfo
    {
        public float scale;
    }

    [Serializable]
    public class glTFPbrMetallicRoughness: IJsonSerializable
    {
        public glTFTextureInfo baseColorTexture = null;
        public float[] baseColorFactor;
        public glTFTextureInfo metallicRoughnessTexture = null;
        public float metallicFactor;
        public float roughnessFactor;

        // empty schemas
        public object extensions;
        public object extras;

        public string ToJson()
        {
            var f = new JsonFormatter();
            f.BeginMap();
            if (baseColorTexture != null)
            {
                f.KeyValue(() => baseColorTexture);
            }
            if (baseColorFactor != null)
            {
                f.KeyValue(() => baseColorFactor);
            }
            if (metallicRoughnessTexture != null)
            {
                f.KeyValue(() => metallicRoughnessTexture);
            }
            f.KeyValue(() => metallicFactor);
            f.KeyValue(() => roughnessFactor);
            f.EndMap();
            return f.ToString();
        }
    }


    [Serializable]
    public class glTFMaterial: IJsonSerializable
    {
        public string name;
        public glTFPbrMetallicRoughness pbrMetallicRoughness;
        public glTFMaterialNormalTextureInfo normalTexture = null;
        public glTFMaterialOcclusionTextureInfo occlusionTexture = null;
        public glTFTextureInfo emissiveTexture = null;
        public float[] emissiveFactor;
        public string alphaMode;
        public float alphaCutoff=0.5f;
        public bool doubleSided;

        // empty schemas
        public object extensions;
        public object extras;

        public string ToJson()
        {
            var f = new JsonFormatter();
            f.BeginMap();
            if (!String.IsNullOrEmpty(name))
            {
                f.Key("name"); f.Value(name);
            }
            if (pbrMetallicRoughness != null)
            {
                f.Key("pbrMetallicRoughness"); f.Value(pbrMetallicRoughness);
            }
            if (normalTexture != null)
            {
                f.Key("normalTexture"); f.Value(normalTexture);
            }
            if (occlusionTexture != null)
            {
                f.Key("occlusionTexture"); f.Value(occlusionTexture);
            }
            if (emissiveTexture != null)
            {
                f.Key("emissiveTexture"); f.Value(emissiveTexture);
            }
            if (emissiveFactor != null)
            {
                f.Key("emissiveFactor"); f.Value(emissiveFactor);
            }
            f.EndMap();
            return f.ToString();
        }
    }
}
