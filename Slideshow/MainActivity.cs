using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using System.IO;
using System.Linq;
using System.Threading;

namespace Slideshow {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // New code will go here

            // 現状SDカードのDCIMフォルダのパスが判定出来ないので、強引に実パスに変換している TODO: 修正
            var _di = new DirectoryInfo(getPathForDCIM().Replace("/emulated", "").Replace("/0/","/0000-0000/"));
            // ※この環境でのパス /storage/0000-0000/DCIM/abcdefghi.jpg

            // ファイルの一覧を取得
            var _filePathList = _di.GetFiles().Where(file => file.Name.EndsWith(".jpg")).ToList();

            //fileList.ForEach(file => translatedPhoneWord.Text = $"Path: {file.Name}");

            // System.Threading.Timer(TimerCallback callback,Object state,int dueTime,int period)
            // callback コールバック関数
            // state コールバックで使用される情報
            // dueTime　開始までの遅延 (ミリ秒)
            // period インターバル (ミリ秒)
            int _idx = 0;
            var timer = new Timer((o) => RunOnUiThread(() => {
                // 一枚ずつ画像表示
                Bitmap _bitmap = BitmapFactory.DecodeFile(_filePathList[_idx++].ToString());
                ImageView _imageView = FindViewById<ImageView>(Resource.Id.MainImageView);
                _imageView.SetImageBitmap(_bitmap);
                _bitmap.Dispose();
                if (_idx == _filePathList.Count) { _idx = 0; }
            }), null, 0, 5000); // タイマーで5秒ごとに
        }

        private static string getPathForDCIM() {
            // DCIM フォルダを取得してる ※必ずしもSDカードではない
            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDcim).Path;
        }
    }
}