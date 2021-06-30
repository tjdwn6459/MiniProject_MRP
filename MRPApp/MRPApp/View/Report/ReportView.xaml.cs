using System;
using System.Windows;
using System.Windows.Controls;

namespace MRPApp.View.Report
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ReportView : Page
    {
        public ReportView()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitControls();
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 ReportView Loaded : {ex}");
                throw ex;
            }
        }

        private void InitControls()
        {
            DtpSearchStartDate.SelectedDate = DateTime.Now.AddDays(-7);//1주일전의 데이터가 들어간다
            DtpSearchEndDate.SelectedDate = DateTime.Now; //오늘 날짜 
        }

        private void BtnEditMyAccount_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new EditAccount()); // 계정정보 수정 화면으로 변경
        }

        private void Btnsearch_Click(object sender, RoutedEventArgs e)
        {
            if(IsValidInputs())
            {
                MessageBox.Show("검색 시작!!");
            }
        }

        private bool IsValidInputs()
        {
            var result = true;


            //검증은 to be continue,,,
            return result;
        }
    }
}
