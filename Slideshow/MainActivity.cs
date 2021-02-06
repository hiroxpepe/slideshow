using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using static Android.Views.View;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Slideshow {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {

        int idx = 0;

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

            var _di = new DirectoryInfo($"/storage/emulated/0/Download"); // TODO: 選択出来るように
                                                                          // /storage/emulated/0/Download
            // TODO: SDカードを取得するには？

            // JPEGファイルの一覧を取得
            var _filePathList = _di.GetFiles()
                .Where(x => x.Name.EndsWith(".JPG") || x.Name.EndsWith(".jpg"))
                .OrderBy(x => x.CreationTime)
                .ToList();

            // System.Threading.Timer(TimerCallback callback,Object state,int dueTime,int period)
            // callback コールバック関数
            // state コールバックで使用される情報
            // dueTime　開始までの遅延 (ミリ秒)
            // period インターバル (ミリ秒)
            //int _idx = 0;
            var _timer = new Timer(x => RunOnUiThread(() => {
                    Bitmap _bitmap = BitmapFactory.DecodeFile(_filePathList[idx++].ToString()); // 一枚ずつ画像表示
                    ImageView _imageView = FindViewById<ImageView>(Resource.Id.MainImageView);
                    _imageView.SetImageBitmap(_bitmap);
                    _bitmap.Dispose();
                    if (idx == _filePathList.Count) { 
                        idx = 0;
                    }
                }),
                null,
                0,
                2000 // タイマーで2秒ごとに
            );

            ImageView _imageView = FindViewById<ImageView>(Resource.Id.MainImageView);
            _imageView.SetOnTouchListener(new OnTouchListener());

        }

        private static string getPathForDCIM() {
            // DCIM フォルダを取得してる ※必ずしもSDカードではない
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).Path;
        }

        public string Hoge() {
            return "hoge";
        }

        public class OnTouchListener : Java.Lang.Object, View.IOnTouchListener {

            bool mIsPagerViewTouchDown = false;
            int mPreviousTouchPointX = 0;

            public bool OnTouch(View v, MotionEvent e) {
                /* do stuff */
                float touchX = e.GetX();

                View _p = (View) v.Parent;
                MainActivity _o = (MainActivity) _p.Context;
                var _tmp = _o.Hoge();

                switch (e.Action) {
                    case MotionEventActions.Down:
                        mPreviousTouchPointX = (int) touchX;
                        break;
                    case MotionEventActions.Up:
                        float dx = touchX - mPreviousTouchPointX;
                        // TouchDown時のタッチ座標とTouchUp時の座標を比較しどちらにフリックしたか判定
                        if ((Math.Abs(dx) > 1)) {
                            if (dx > 0) {
                                Log.Info($"右へフリック: {dx}");
                            }
                        } else {
                            Log.Info($"左へフリック: {dx}");
                        }
                        break;
                    default:
                        break;
                }
                mPreviousTouchPointX = (int) touchX;
                return false;
            }
        }
    }
}