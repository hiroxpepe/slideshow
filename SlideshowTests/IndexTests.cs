
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Slideshow.Tests {
    [TestClass()]
    public class IndexTests {

        [TestMethod()]
        public void IncrementTest() {
            var _target = new Index(4); // 4枚の画像を想定: 0 ～ 3
            Assert.AreEqual(_target.Idx, 0);
            _target.Increment();
            Assert.AreEqual(_target.Idx, 1);
            _target.Increment();
            _target.Increment();
            Assert.AreEqual(_target.Idx, 3);
            _target.Increment(); // ここで最大値を超えるので 0 にリセット
            Assert.AreEqual(_target.Idx, 0);
            _target.Increment();
            Assert.AreEqual(_target.Idx, 1);
            _target.Increment();
            _target.Increment();
            Assert.AreEqual(_target.Idx, 3);
            _target.Increment(); // ここで最大値を超えるので 0 にリセット
            Assert.AreEqual(_target.Idx, 0);
        }

        [TestMethod()]
        public void DecrementTest() {
            var _target = new Index(4); // 4枚の画像を想定: 0 ～ 3
            Assert.AreEqual(_target.Idx, 0);
            _target.Decrement(); // マイナスではなく最大値を設定
            Assert.AreEqual(_target.Idx, 3);
            _target.Decrement();
            _target.Decrement();
            Assert.AreEqual(_target.Idx, 1);
            _target.Decrement();
            Assert.AreEqual(_target.Idx, 0);
            _target.Decrement(); // マイナスではなく最大値を設定
            Assert.AreEqual(_target.Idx, 3);
            _target.Decrement();
            _target.Decrement();
            Assert.AreEqual(_target.Idx, 1);
        }
    }
}