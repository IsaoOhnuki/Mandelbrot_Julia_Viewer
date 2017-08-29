using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Controls
{
    public interface IColorPickerDependencyService
    {
        // モデル名を取得するメソッド
        string GetModel();

        // OSのバージョン文字列を返すプロパティ（読み取り専用）
        string OsVersion { get; }

        void ShowPicker(Color SelectedColor);
        Color SelectedColor { get; set; }
    }
}
