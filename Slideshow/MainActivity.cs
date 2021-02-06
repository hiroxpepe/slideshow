using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Slideshow {

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Index index;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public MainActivity() {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Increment() {
            index.Increment();
        }

        public void Decrement() {
            index.Decrement();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

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
                .Where(x => x.Name.EndsWith(".JPG") || x.Name.EndsWith(".jpg") || x.Name.EndsWith(".PNG") || x.Name.EndsWith(".png"))
                .OrderBy(x => x.CreationTime)
                .ToList();

            // index カウンタオブジェクト生成
            index = new Index(_filePathList.Count);

            // System.Threading.Timer(TimerCallback callback,Object state,int dueTime,int period)
            // callback コールバック関数
            // state コールバックで使用される情報
            // dueTime　開始までの遅延 (ミリ秒)
            // period インターバル (ミリ秒)
            //int _idx = 0;
            var _timer = new Timer(x => RunOnUiThread(() => {
                    Bitmap _bitmap = BitmapFactory.DecodeFile(_filePathList[index.Idx].ToString()); // 一枚ずつ画像表示
                    ImageView _imageView = FindViewById<ImageView>(Resource.Id.MainImageView);
                    _imageView.SetImageBitmap(_bitmap);
                    _bitmap.Dispose();
                    index.Increment();
                }),
                null,
                0,
                1500 // タイマーで1.5秒ごとに
            );

            ImageView _imageView = FindViewById<ImageView>(Resource.Id.MainImageView);
            _imageView.SetOnTouchListener(new OnTouchListener());

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        private static string getPathForDCIM() {
            // DCIM フォルダを取得してる ※必ずしもSDカードではない
            return Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim).Path;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class OnTouchListener : Java.Lang.Object, View.IOnTouchListener {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int previousTouchPointX = 0;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            public bool OnTouch(View v, MotionEvent e) {
                /* do stuff */
                float _touchPointX = e.GetX();
                Log.Info($"_touchPointX: {_touchPointX}");
                MainActivity _activity = getActivity(v);
                switch (e.Action) {
                    case MotionEventActions.Down:
                        previousTouchPointX = (int) _touchPointX;
                        break;
                    case MotionEventActions.Up:
                        float _dx = _touchPointX - previousTouchPointX;
                        if ((Math.Abs(_dx) > 1)) { // TouchDown時のタッチ座標とTouchUp時の座標を比較しどちらにフリックしたか判定
                            if (_dx > 0) {
                                Log.Info($"touchX: {_touchPointX} 右へフリック: {_dx}");
                                _activity.Decrement();
                            } else {
                                Log.Info($"touchX: {_touchPointX} 左へフリック: {_dx}");
                                _activity.Increment();
                            }
                        }
                        break;
                    default:
                        break;
                }
                return true;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Methods [verb]

            MainActivity getActivity(View view) {
                View _view = (View) view.Parent;
                return (MainActivity) _view.Context;
            }
        }
    }

    /// <summary>
    /// 画像のインデックスを保持するクラス
    /// </summary>
    public class Index {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int idx; // List の中の画像ファイルの index

        int count; // List の中の画像ファイルの数

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Index(int count) {
            this.idx = 0;
            this.count = count;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// 画像ファイルの index 値
        /// </summary>
        public int Idx {
            get => idx;
            private set => idx = value;
        }

        public bool HasNext {
            get => false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Increment() {
            if (idx == count) {
                idx = 0; // 最小値
                return;
            }
            idx++;
        }

        public void Decrement() {
            if (idx == 0) {
                idx = count; // 最大値
                return;
            }
            idx--;
        }
    }
}