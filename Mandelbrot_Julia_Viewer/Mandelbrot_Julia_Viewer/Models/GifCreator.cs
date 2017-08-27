using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    // http://www.tohoho-web.com/wwwgif.htm GIFフォーマットの詳細

    /*
    Signature(3B) = "GIF"
    Version(3B) = "87a" or "89a"
    Logical Screen Width(2B)
    Logical Screen Height(2B)
    GCTF(1b)	CR(3b)	SF(1b)	SGCT(3b)
    Background Color Index(1B)
    Pixel Aspect Ratio(1B)

    Global Color Table(0～255×3B)
     */

    public class GifCreator
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct GifHeader
        {
            byte G;
            byte I;
            byte F;
            byte V;
            byte E;
            byte R;
            short Width;
            short Height;
            byte ColorAttribute;
            byte BackGroundColorIndex;
            byte PixelAspectRatio;
        }
        public static void Create(short Width, short Height)
        {
            System.IO.MemoryStream gif = new System.IO.MemoryStream();
            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(gif);
            // gifヘッダー
            bw.Write((byte)'G');
            bw.Write((byte)'I');
            bw.Write((byte)'F');
            bw.Write((byte)'8');
            bw.Write((byte)'7');
            bw.Write((byte)'a');
            bw.Write(Width);
            bw.Write(Height);
            bw.Write((byte)0);  // カラーテーブルなし
            bw.Write((byte)0);  // バックグラウンドカラーインデクス
            bw.Write((byte)49); // 縦横比 (n + 15)/64
            // カラーテーブルブロック
            // イメージブロック
            bw.Write((byte)0x2c);
            bw.Write((short)0);
            bw.Write((short)0);
            bw.Write(Width);
            bw.Write(Height);
            bw.Write((byte)0);  // カラーテーブルなし

        }
    }
}
