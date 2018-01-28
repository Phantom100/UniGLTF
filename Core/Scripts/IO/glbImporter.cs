﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_2017_OR_NEWER
using UnityEditor.Experimental.AssetImporters;
#endif

namespace UniGLTF
{
#if UNITY_2017_OR_NEWER && USE_UNIGLTF_SCRIPTEDIMPORTER
    [ScriptedImporter(1, "glb")]
#endif
    public class glbImporter
#if UNITY_2017_OR_NEWER
        : ScriptedImporter
#endif
    {
        public const string GLB_MAGIC = "glTF";
        public const float GLB_VERSION = 2.0f;

#if UNITY_EDITOR
        [MenuItem("Assets/gltf/import")]
        public static void ImportMenu()
        {
            var path = UnityEditor.EditorUtility.OpenFilePanel("open gltf", "", "gltf,glb");
            if (!string.IsNullOrEmpty(path))
            {
                Debug.Log(path);
                var bytes = File.ReadAllBytes(path);
                var ext = Path.GetExtension(path).ToLower();
                switch (ext)
                {
                    case ".gltf":
                        {
                            var json = Encoding.UTF8.GetString(bytes);
                            var root = gltfImporter.Import(path, json, new ArraySegment<byte>(), false);
                            if (root == null)
                            {
                                return;
                            }
                            root.name = Path.GetFileNameWithoutExtension(path);
                        }
                        break;

                    case ".glb":
                        {
                            var root = glbImporter.Import(path, bytes, false);
                            if (root == null)
                            {
                                return;
                            }
                            root.name = Path.GetFileNameWithoutExtension(path);
                        }
                        break;

                    default:
                        Debug.LogWarningFormat("unknown ext: {0}", path);
                        break;
                }
            }
        }
#endif

#if UNITY_2017_OR_NEWER
        public override void OnImportAsset(AssetImportContext ctx)
        {
            Debug.LogFormat("## glbImporter ##: {0}", ctx.assetPath);

            var bytes = File.ReadAllBytes(ctx.assetPath);

            Import(new gltfImporter.Context(ctx), bytes);
        }
#endif

        public static GlbChunkType ToChunkType(string src)
        {
            switch(src)
            {
                case "BIN":
                    return GlbChunkType.BIN;

                case "JSON":
                    return GlbChunkType.JSON;

                default:
                    throw new FormatException("unknown chunk type: " + src);
            }
        }

        public static GameObject Import(string path, Byte[] bytes, bool isPrefab)
        {
            int pos = 0;
            if(Encoding.ASCII.GetString(bytes, 0, 4) != GLB_MAGIC)
            {
                throw new Exception("invalid magic");
            }
            pos += 4;

            var version = BitConverter.ToUInt32(bytes, pos);
            if (version != GLB_VERSION)
            {
                Debug.LogWarningFormat("{0}: unknown version: {1}", path, version);
                return null;
            }
            pos += 4;

            //var totalLength = BitConverter.ToUInt32(bytes, pos);
            pos += 4;

            var chunks = new List<GlbChunk>();
            while(pos<bytes.Length)
            {
                var chunkDataSize = BitConverter.ToInt32(bytes, pos);
                pos += 4;

                //var type = (GlbChunkType)BitConverter.ToUInt32(bytes, pos);
                var chunkTypeBytes = bytes.Skip(pos).Take(4).Where(x => x != 0).ToArray();
                var chunkTypeStr = Encoding.ASCII.GetString(chunkTypeBytes);
                var type = ToChunkType(chunkTypeStr);
                pos += 4;

                chunks.Add(new GlbChunk
                {
                    ChunkType = type,
                    Bytes = new ArraySegment<byte>(bytes, (int)pos, (int)chunkDataSize)
                });

                pos += chunkDataSize;
            }

            if(chunks.Count!=2)
            {
                throw new Exception("unknown chunk count: "+chunks.Count);
            }

            if (chunks[0].ChunkType != GlbChunkType.JSON)
            {
                throw new Exception("chunk 0 is not JSON");
            }

            if (chunks[1].ChunkType != GlbChunkType.BIN)
            {
                throw new Exception("chunk 1 is not BIN");
            }

            var jsonBytes = chunks[0].Bytes;
            var json = Encoding.UTF8.GetString(jsonBytes.Array, jsonBytes.Offset, jsonBytes.Count);

            return gltfImporter.Import(path,
                json, 
                chunks[1].Bytes,
                isPrefab);
        }
    }
}