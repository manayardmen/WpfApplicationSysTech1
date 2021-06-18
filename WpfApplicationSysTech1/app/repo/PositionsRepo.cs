using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApplicationSysTech1.app.models;

namespace WpfApplicationSysTech1.app.repo
{
    // Операции над таблицей Positions
    public static class PositionsRepo
    {
        public static int GetBasicPositionId()
        {
            var context = AppMain.Instance.Context;
            var appSettings = AppMain.Instance.AppSettings;

            var result = -1;

            try
            {
                var position = context.Positions.FirstOrDefault(p => p.Name == appSettings.BasicPositionName);
                if (position != null) result = position.Id;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
            }

            return result;
        }

        public static int GetEditorPositionId()
        {
            var context = AppMain.Instance.Context;
            var appSettings = AppMain.Instance.AppSettings;

            var result = -1;

            try
            {
                var position = context.Positions.FirstOrDefault(p => p.Name == appSettings.EditorPositionName);
                if (position != null) result = position.Id;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
            }


            return result;
        }

        public static List<Position> GetPositionsList()
        {
            var context = AppMain.Instance.Context;
            var result = context.Positions.ToList();
            return result;
        }
    }
}
