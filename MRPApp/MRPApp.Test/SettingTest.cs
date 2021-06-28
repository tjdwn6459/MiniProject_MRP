using Microsoft.VisualStudio.TestTools.UnitTesting;
using MRPApp.View.Setting;
using System;
using System.Linq;


namespace MRPApp.Test
{

    //Db사에 중복된 데이터 있는지 테스트 
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void IsValidInputsTest()
        {
            var expectVal = true; //예상값
            var inputCode = "PC010001"; //db상에 있는 값

            var code = Logic.DataAccess.Getsettings().Where(d => d.BasicCode.Contains(inputCode)).FirstOrDefault();
            var realVal = (code != null) ? 1 : 0;

            Assert.AreEqual(expectVal, realVal); //값이 같으면 pass, 다르면 fail

        }

        [TestMethod]
        public void IsCodeSearchead()
        {
            var exceptVal = 2;//예상값
            var inputCode = "설비";

            var realVal = Logic.DataAccess.Getsettings().Where(d => d.CodeName.Contains(inputCode)).Count();

            Assert.AreEqual(exceptVal, realVal);
        }

        [TestMethod]
        public void IsEmailCorrect()
        {
            //var inputEmail = "tjdwn6459@naver.com";
            //Assert.IsTrue(Commons.IsValidEmail(inputEmail));
        }

        [TestMethod]
        public void IsEmailIncorrect()
        {
            //var inputEmail = "tjdwn6459@naver";
            //Assert.IsFalse(Commons.IsValidEmail(inputEmail));
        }

    }
}

