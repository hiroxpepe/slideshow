using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.IO;
using System.Linq;
using System.Threading;
using Android.Util;

namespace Slideshow {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // New code will go here

            // 現状SDカードのDCIMフォルダのパスが判定出来ないので、強引に実パスに変換している TODO: 修正
            //var _di = new DirectoryInfo(getPathForDCIM().Replace("/emulated", "").Replace("/0/","/0000-0000/"));
            // ※この環境でのパス /storage/0000-0000/DCIM/abcdefghi.jpg
            // ※この環境でのパス /storage/3838-6330/DCIM/abcdefghi.jpg

            //var _di = new DirectoryInfo($"{getPathForDCIM()}/100SHARP");
            // /storage/emulated/0/DCIM/100SHARP

            var _di = new DirectoryInfo($"/storage/emulated/0/Download");
            // /storage/emulated/0/Download

            // ファイルの一覧を取得
            var _filePathList = _di.GetFiles()
                .Where(x => x.Name.EndsWith(".JPG") || x.Name.EndsWith(".jpg"))
                .OrderBy(x => x.CreationTime)
                .ToList();

            //fileList.ForEach(file => translatedPhoneWord.Text = $"Path: {file.Name}");

            // System.Threading.Timer(TimerCallback callback,Object state,int dueTime,int period)
            // callback コールバック関数
            // state コールバックで使用される情報
            // dueTime　開始までの遅延 (ミリ秒)
            // period インターバル (ミリ秒)
            int _idx = 0;
            var _timer = new Timer(x => RunOnUiThread(() => {
                // 一枚ずつ画像表示
                Bitmap _bitmap = BitmapFactory.DecodeFile(_filePathList[_idx++].ToString());
                ImageView _imageView = FindViewById<ImageView>(Resource.Id.MainImageView);

                //DisplayMetrics _mets = new DisplayMetrics();
                //double viewWidthToBitmapWidthRatio = (double) _mets.WidthPixels / (double) _bitmap.Width;
                //_imageView.LayoutParameters.Height = (int) (_bitmap.Height * viewWidthToBitmapWidthRatio);

                _imageView.SetImageBitmap(_bitmap);
                _bitmap.Dispose();
                if (_idx == _filePathList.Count) { _idx = 0; }
            }), null, 0, 2000); // タイマーで2秒ごとに
        }

        // TODO: SDカードを取得するには？

        private static string getPathForDCIM() {
            // DCIM フォルダを取得してる ※必ずしもSDカードではない
            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDcim).Path;
        }
    }
}