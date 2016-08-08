using UnityEngine;

namespace Assets.MoreFun.Utils
{
    public class RenderUtil
    {
        public static bool DoesTextureFormatHaveAlphaChannel(TextureFormat fmt)
        {
            switch(fmt)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.ARGB32:
                case TextureFormat.ARGB4444:
                //case TextureFormat.ASTC_RGBA_10x10:
                //case TextureFormat.ASTC_RGBA_12x12:
                //case TextureFormat.ASTC_RGBA_4x4:
                //case TextureFormat.ASTC_RGBA_5x5:
                //case TextureFormat.ASTC_RGBA_6x6:
                //case TextureFormat.ASTC_RGBA_8x8:
                case TextureFormat.ATC_RGBA8:
                //case TextureFormat.ATF_RGBA_JPG:
                case TextureFormat.BGRA32:
                case TextureFormat.DXT5:
                //case TextureFormat.ETC2_RGBA1:
                //case TextureFormat.ETC2_RGBA8:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.RGBA32:
                case TextureFormat.RGBA4444:
                    return true;

                default:
                    return false;
            }
        }

    }
}
