﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MRPApp.View.Process
{
    /// <summary>
    /// ProcessView.xaml에 대한 상호 작용 논리
    /// 1. 공정계획서 오늘의 생산계획 일정 불러옴
    /// 2. 없으면 에러표시, 시작버튼 클릭하지 못하게 만듬
    /// 3. 있으면 오늘의 날짜르르 표시, 시작버튼 활성화
    /// 4. 시작버튼 클릭시 새 공정을 생성, DB에 입력
    ///     공정코드 : PRC20210618001 ( PRC + yyyy+MM+dd+NNN)
    /// 5. 공정처리 애니메이션 시작
    /// 6. 로드타임 후 애니메이션 중지
    /// 7. 센서링값 리턴될때까지 대기
    /// 8. 센서링 결과값 따라서 생산품 색상 변경
    /// 9. 현재 공정의 Db값 업데이트
    /// 10.결과 레이블 값 수정/표시
    /// </summary>
    public partial class ProcessView : Page
    {
        //금일 일정
        private Model.Schedules currSchedule;
        public ProcessView()
        {
            InitializeComponent();
        }

        private  async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var today = DateTime.Now.ToString("yyyy-MM-dd");
                currSchedule = Logic.DataAccess.GetSchedules().Where(s => s.PlantCode.Equals(Commons.PLANTCODE))
                                    .Where(s => s.SchDate.Equals(DateTime.Parse(today))).FirstOrDefault();

                if(currSchedule == null)
                {
                    await Commons.ShowMessageAsync("공정", "공정계획이 없습니다. 계획일정을 먼저 입력하세요.");
                    LblProcessDate.Content = string.Empty;
                    LblSchLoadTime.Content = "None";
                    LblSchAmount.Content = "None";
                    BtnStartProcess.IsEnabled = false;

                    //TODO : 시작버튼 Disable

                    return;
                }
                else
                {
                    //공정계획 표시
                    MessageBox.Show($"{today} 공정 시작합니다.");
                    LblProcessDate.Content = currSchedule.SchDate.ToString("yyyy년 MM월 dd일");
                    LblSchLoadTime.Content = $"{currSchedule.SchLoadTime}초";
                    LblSchAmount.Content = $"{currSchedule.SchAmount}개";
                    BtnStartProcess.IsEnabled = true;
                }
             
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 ProcessView Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnStartProcess_Click(object sender, RoutedEventArgs e)
        {

            //기어 애니메이션 속성
            DoubleAnimation da = new DoubleAnimation();
            da.From = 0;
            da.To = 360;
            da.Duration = TimeSpan.FromSeconds(currSchedule.SchLoadTime); //일정 계획로드타임
            //da.RepeatBehavior = RepeatBehavior.Forever;

            RotateTransform rt = new RotateTransform();
            Gear1.RenderTransform = rt;
            Gear1.RenderTransformOrigin = new Point(0.5, 0.5);
            Gear2.RenderTransform = rt;
            Gear2.RenderTransformOrigin = new Point(0.5, 0.5);
         

            rt.BeginAnimation(RotateTransform.AngleProperty, da);

            //제픔 애니메이션 속성
            DoubleAnimation ma = new DoubleAnimation();
            ma.From = 119; //시작 위치점
            ma.To = 557;//끝나는 위치점
            ma.Duration = TimeSpan.FromSeconds(currSchedule.SchLoadTime);
            //ma.AccelerationRatio = 0.5;
            /*ma.AutoReverse = true; *///다시 제자리로 돌아옴

            Product.BeginAnimation(Canvas.LeftProperty, ma);

                                                                                       
        }
    }
}