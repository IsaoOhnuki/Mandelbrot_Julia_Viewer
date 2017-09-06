using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BitmapCreator
    {
        public static Task<byte[]> Create(short Width, short Height, byte[] data)
        {
            return Task<byte[]>.Run(() => {
                System.IO.MemoryStream bmp = new System.IO.MemoryStream();
                System.IO.BinaryWriter bw = new System.IO.BinaryWriter(bmp);

                // OS/2Bitmap
                // bitmapヘッダ
                int widthFiller = (Width * 3) % 4;
                widthFiller = widthFiller == 0 ? 0 : 4 - widthFiller;
                bw.Write((byte)'B');
                bw.Write((byte)'M');
                bw.Write((int)14 + 12 + (Width * 3 + widthFiller) * Height);
                bw.Write((short)0);
                bw.Write((short)0);
                bw.Write((int)14 + 12);
                // 情報ヘッダ
                bw.Write((int)12);
                bw.Write((short)Width);
                bw.Write((short)Height);
                bw.Write((short)1);
                bw.Write((short)24);
                // イメージブロック
                for (int h = Height - 1; h >= 0; --h)
                {
                    for (int w = 0; w < Width * 4; w += 4)
                    {
                        bw.Write(data[h * Width * 4 + w]); // B値
                        bw.Write(data[h * Width * 4 + w + 1]); // G値
                        bw.Write(data[h * Width * 4 + w + 2]); // R値
                        //bw.Write(data[h * Width * 4 + w + 3]); α値は捨てる
                    }
                    for (int w = 0; w < widthFiller; ++w)
                    {
                        bw.Write((byte)0); // 4バイト境界調整
                    }
                }
                //// WindowsBitmap
                //// bitmapヘッダ
                //bw.Write((byte)'B');
                //bw.Write((byte)'M');
                //bw.Write((int)14 + 40 + Width * 4 * Height);
                //bw.Write((short)0);
                //bw.Write((short)0);
                //bw.Write((int)14 + 40);
                //// 情報ヘッダ
                //bw.Write((int)40);
                //bw.Write((int)Width);
                //bw.Write((int)Height);
                //bw.Write((short)1);
                //bw.Write((short)32);
                //bw.Write((int)0);
                //bw.Write((int)Width * Height * 4);
                //bw.Write((int)3780);
                //bw.Write((int)3780);
                //bw.Write((int)0);
                //bw.Write((int)0);
                //// イメージブロック
                //for (int h = Height - 1; h >= 0; --h)
                //{
                //    for (int w = 0; w < Width * 4; w += 4)
                //    {
                //        bw.Write(data[h * Width * 4 + w]); // B値
                //        bw.Write(data[h * Width * 4 + w + 1]); // G値
                //        bw.Write(data[h * Width * 4 + w + 2]); // R値
                //        bw.Write(data[h * Width * 4 + w + 3]); // α値
                //    }
                //}
                bw.Flush();
                bw.Dispose();
                return bmp.ToArray();
            });
        }
    }
}
