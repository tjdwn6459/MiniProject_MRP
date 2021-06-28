using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace MRPApp.View.Schedule
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScheduleList : Page
    {
        public ScheduleList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadControlData(); //콤보박스 데이터 로딩
                LoadGridData(); //테이블데이터 그리드 표시
                InitErrorMessage();
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 StoreList Loaded : {ex}");
                throw ex;
            }
        }

        private void LoadControlData()
        {
            var plantCodes = Logic.DataAccess.Getsettings().Where(c => c.BasicCode.Contains("PC01")).ToList(); //Getsettings에 있는 데이터중 PC01 포함된 리스트를  가죠옴
            CboPlantCode.ItemsSource = plantCodes;
            CboGridPlantCode.ItemsSource = plantCodes;


            var facilityIds = Logic.DataAccess.Getsettings().Where(c => c.BasicCode.Contains("FAC1")).ToList();
            CboSchFacilityID.ItemsSource = facilityIds;
        }

        private void InitErrorMessage()
        {
            LblPlantCode.Visibility = LblSchDate.Visibility = LblSchLoadTime.Visibility
                 = LblSchStartTime.Visibility = LblSchEndTime.Visibility =
                 LblSchFacilityID.Visibility = LblSchAmount.Visibility = Visibility.Hidden;
        }

        private void LoadGridData()
        {
            List<Model.Schedules> list = Logic.DataAccess.GetSchedules();
            this.DataContext = list;
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //NavigationService.Navigate(new EditUser());
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditUser_Click : {ex}");
                throw ex;
            }
        }



        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {
            if (GrdData.SelectedItem == null)
            {
                Commons.ShowMessageAsync("창고수정", "수정할 창고를 선택하세요");
                return;
            }

            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditStore_Click : {ex}");
                throw ex;
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidUpdates() != true) return;

            var item = GrdData.SelectedItem as Model.Schedules;
            item.PlantCode = CboPlantCode.SelectedValue.ToString();
            item.SchDate = DateTime.Parse(DtpSchDate.Text);
            item.SchLoadTime = int.Parse(TxtSchLoadTime.Text);
            if (TmpSchStartTime.SelectedDateTime != null)
                item.SchStartTime = TmpSchStartTime.SelectedDateTime.Value.TimeOfDay;
            if (TmpSchEndTime.SelectedDateTime != null)
                item.SchEnd = TmpSchEndTime.SelectedDateTime.Value.TimeOfDay;


            item.SchFacilityID = CboSchFacilityID.SelectedValue.ToString();
            item.SchAmount = (int)NudSchAmount.Value;

            item.ModDate = DateTime.Now;
            item.ModID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSchedule(item);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 수정시 오류발생");
                    await Commons.ShowMessageAsync("오류", "데이터 수정실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 수정 성공!! : {item.SchIdx}"); //로구
                    ClearInputs();
                    LoadGridData(); //성공하면 이 메소드가 실행된다 
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 {ex}");
            }
        }

        //클릭했을때 신규버튼이 있을때 -> 컨트롤에 대해 초기화 한다 
        private void ClearInputs()
        {

            TxtSchIdx.Text = ""; //값을 비워준다
            CboPlantCode.SelectedItem = null;
            DtpSchDate.Text = "";
            TxtSchLoadTime.Text = "";
            TmpSchStartTime.SelectedDateTime = null;
            TmpSchEndTime.SelectedDateTime = null;
            CboSchFacilityID.SelectedItem = null;
            NudSchAmount.Value = 0;
            CboPlantCode.Focus(); //새로시작시 여기서 부터 입력이 된다

        }

        private async void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidInputs() != true) return;


            var item = new Model.Schedules();
            item.PlantCode = CboPlantCode.SelectedValue.ToString();
            item.SchDate = DateTime.Parse(DtpSchDate.Text);
            item.SchLoadTime = int.Parse(TxtSchLoadTime.Text);
            if (TmpSchStartTime.SelectedDateTime != null)
                item.SchStartTime = TmpSchStartTime.SelectedDateTime.Value.TimeOfDay;
            if (TmpSchEndTime.SelectedDateTime != null)
                item.SchEnd = TmpSchEndTime.SelectedDateTime.Value.TimeOfDay;


            item.SchFacilityID = CboSchFacilityID.SelectedValue.ToString();
            item.SchAmount = (int)NudSchAmount.Value;

            item.RegDate = DateTime.Now;
            item.RegID = "MRP";

            try
            {
                var result = Logic.DataAccess.SetSchedule(item);
                if (result == 0)
                {
                    Commons.LOGGER.Error("데이터 입력시 오류발생");
                    await Commons.ShowMessageAsync("오류", "데이터 입력실패!!");
                }
                else
                {
                    Commons.LOGGER.Info($"데이터 입력 성공!! : {item.SchIdx}"); //로그
                    //ClearInputs();
                    LoadGridData(); //성공하면 이 메소드가 실행된다 
                }
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 {ex}");
            }
        }


        //입력데이터 검증 메소드
        public bool IsValidInputs()
        {
            var isValid = true;
            InitErrorMessage();

            if (CboPlantCode.SelectedValue == null)
            {
                LblPlantCode.Visibility = Visibility.Visible;
                LblPlantCode.Text = "공장을 선택하세요.";
                isValid = false;

            }

            if (string.IsNullOrEmpty(DtpSchDate.Text))
            {
                LblSchDate.Visibility = Visibility.Visible;
                LblSchDate.Text = "공정일을 입력하세요.";
                isValid = false;
            }

            //if (CboPlantCode.SelectedValue != null && !string.IsNullOrEmpty(DtpSchDate.Text))
            //{

            //    //공장별로 공정일이 이미 Db에 값이 있으면 입력되면 안됨
            //    //PC010001(수원) 2021-06-24 이미 있는데 또 넣으려는거 막는다
            //    var result = Logic.DataAccess.GetSchedules().Where(s => s.PlantCode.Equals(CboPlantCode.SelectedValue.ToString()))
            //       .Where(d => d.SchDate.Equals(DateTime.Parse(DtpSearchDate.Text))).Count();
            //    if (result > 0)
            //    {
            //        LblSchDate.Visibility = Visibility.Visible;
            //        LblSchDate.Text = "해당공장 공정일에 계획이 이미 있습니다.";
            //        isValid = false;
            //    }
            //}


            if (string.IsNullOrEmpty(TxtSchLoadTime.Text))
            {
                LblSchLoadTime.Visibility = Visibility.Visible;
                LblSchLoadTime.Text = "로드타임을 입력하세요.";
                isValid = false;
            }

            if (CboSchFacilityID.SelectedValue == null)
            {
                LblSchFacilityID.Visibility = Visibility.Visible;
                LblSchFacilityID.Text = "공정설비를 선택하세요.";
                isValid = false;
            }

            if (NudSchAmount.Value <= 0)
            {
                LblSchAmount.Visibility = Visibility.Visible;
                LblSchAmount.Text = "계획수량은 0개 이상입니다.";
                isValid = false;
            }

            return isValid;
        }



        //수정데이터 검증 메소드
        public bool IsValidUpdates()
        {
            var isValid = true;
            InitErrorMessage();

            if (CboPlantCode.SelectedValue == null)
            {
                LblPlantCode.Visibility = Visibility.Visible;
                LblPlantCode.Text = "공장을 선택하세요.";
                isValid = false;

            }

            if (string.IsNullOrEmpty(DtpSchDate.Text))
            {
                LblSchDate.Visibility = Visibility.Visible;
                LblSchDate.Text = "공정일을 입력하세요.";
                isValid = false;
            }


            if (string.IsNullOrEmpty(TxtSchLoadTime.Text))
            {
                LblSchLoadTime.Visibility = Visibility.Visible;
                LblSchLoadTime.Text = "로드타임을 입력하세요.";
                isValid = false;
            }

            if (CboSchFacilityID.SelectedValue == null)
            {
                LblSchFacilityID.Visibility = Visibility.Visible;
                LblSchFacilityID.Text = "공정설비를 선택하세요.";
                isValid = false;
            }

            if (NudSchAmount.Value <= 0)
            {
                LblSchAmount.Visibility = Visibility.Visible;
                LblSchAmount.Text = "계획수량은 0개 이상입니다.";
                isValid = false;
            }

            return isValid;
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            ClearInputs();
        }

        private void Btnsearch_Click(object sender, RoutedEventArgs e)
        {
            var search = DtpSearchDate.Text.Trim();
            var list = Logic.DataAccess.GetSchedules().Where(s => s.SchDate.Equals(DateTime.Parse(search))).ToList();

            this.DataContext = list;

        }

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ClearInputs(); // 완전히 초기화 
            try
            {

                var item = GrdData.SelectedItem as Model.Schedules;
                TxtSchIdx.Text = item.SchIdx.ToString();
                CboPlantCode.SelectedValue = item.PlantCode;
                DtpSchDate.Text = item.SchDate.ToString();
                TxtSchLoadTime.Text = item.SchLoadTime.ToString();
                if (item.SchStartTime != null)
                    TmpSchStartTime.SelectedDateTime = new DateTime(item.SchStartTime.Value.Ticks);
                if (item.SchEnd != null)
                    TmpSchEndTime.SelectedDateTime = new DateTime(item.SchEnd.Value.Ticks);


                CboSchFacilityID.SelectedValue = item.SchFacilityID;
                NudSchAmount.Value = item.SchAmount;


            }
            catch (Exception ex)
            {

                Commons.LOGGER.Error($"예외발생 {ex}");
                ClearInputs();
            }
        }




    }
}
