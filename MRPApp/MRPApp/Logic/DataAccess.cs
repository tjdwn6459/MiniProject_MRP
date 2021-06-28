using MRPApp.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MRPApp.Logic
{
    public class DataAccess
    {
        //셋팅테이블에서 데이터 가져오기
        public static List<Settings> Getsettings()
        {
            List<Model.Settings> settings;

            using (var ctx = new MRPEntities())
                settings = ctx.Settings.ToList(); //SELECT


            return settings;
        }


        internal static int SetSettings(Settings item)
        {
            using (var ctx = new MRPEntities())
            {
                ctx.Settings.AddOrUpdate(item); //INSERT or UPDATE
                return ctx.SaveChanges(); //commit
            }

        }

        internal static int DelSettings(Settings item)
        {
            using (var ctx = new MRPEntities())
            {
                var obj = ctx.Settings.Find(item.BasicCode);
                ctx.Settings.Remove(obj); //DELETE
                return ctx.SaveChanges();
            }
        }

        internal static List<Schedules> GetSchedules()
        {
            List<Model.Schedules> list;

            using (var ctx = new MRPEntities())
                list = ctx.Schedules.ToList(); //SELECT


            return list;
        }

        internal static int SetSchedule(Schedules item)
        {
            using (var ctx = new MRPEntities())
            {
                ctx.Schedules.AddOrUpdate(item); //INSERT or UPDATE
                return ctx.SaveChanges(); //commit
            }
        }
    }
}

