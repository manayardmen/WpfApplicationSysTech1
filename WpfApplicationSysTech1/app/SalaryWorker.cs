using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplicationSysTech1.app.models;
using WpfApplicationSysTech1.app.repo;
using WpfApplicationSysTech1.app.types;
using WpfApplicationSysTech1.Properties;

namespace WpfApplicationSysTech1.app
{
    // Расчет зарплат
    public static class SalaryWorker
    {
        private static List<SalaryOfWorker> _workersSalary;

        private static void CalcSalaryForAllWorkers(DateTime fromDate, DateTime toDate)
        {
            var context = AppMain.Instance.Context;

            // Сбрасываем значения ЗП
            _workersSalary = new List<SalaryOfWorker>();

            CalcSalaryForWorkersWithoutSubWorkers(fromDate, toDate);

            var currentUsers = context.Users.ToList();

            // Если данные о зарплатах не равны числу всех сотрудников
            while (_workersSalary.Count < currentUsers.Count)
            {
                // Ищем тех, у кого в подчинении есть только те, которые есть в массиве _workersSalary
                CalcSalaryForWorkersWithKnownSubs(fromDate, toDate);
            }
        }

        public static double GetSalaryForThisWorker(int workerId, DateTime fromDate, DateTime toDate)
        {
            // Сначала производим вычисления
            CalcSalaryForAllWorkers(fromDate, toDate);

            var result = 0.0;

            var workerSalary = _workersSalary.FirstOrDefault(ws => ws.WorkerId == workerId);
            if (workerSalary != null)
                result = workerSalary.WorkerSalary;
            else
                Console.WriteLine(Resources.SalaryForThisSelectedWorkerNotFound);

            return result;
        }

        // Получить разницу в годах
        private static int GetYearsDifferenceInDates(DateTime a, DateTime b)
        {
            var beginTime = new DateTime(1, 1, 1);

            TimeSpan span = b - a;
            int years = (beginTime + span).Year - 1;

            return years;
        }

        // Сотрудник без подчиненных
        private static double CalcSalaryForEmployee(DateTime fromDate, DateTime toDate)
        {
            var result = 0.0;

            // Просто считаем ЗП для этого сотрудника
            var datesYearsDiff = GetYearsDifferenceInDates(fromDate, toDate);

            var myPositionData = AppMain.Instance.Positions.FirstOrDefault(p => p.Name == "Employee");
            if (myPositionData != null)
            {
                var basicSalary = myPositionData.BaseRate;
                var perYearRate = myPositionData.PerYearRate;
                var maxRate = myPositionData.MaxRate;

                var currentRate = perYearRate * datesYearsDiff + 0.0;

                if (currentRate > maxRate) currentRate = maxRate;

                result = basicSalary + (basicSalary / 100.0) * currentRate;
            }
            else
            {
                Console.WriteLine(Resources.ErrorEmployeePositionNotFound);
            }

            return result;
        }

        // На вход приходит тот сотрудник, у которого в подчинении есть только сотрудники с просчитанной ЗП
        private static double CalcSalaryForManager(int userId, DateTime fromDate, DateTime toDate)
        {
            var result = 0.0;

            var mySubsSalary = 0.0;

            // Нужно получить идентификаторы для подчиненных только первого уровня
            var ids = UsersRepo.GetOnlyMySubWorkers(userId);
            foreach (var id in ids)
            {
                var thisWorkerSalaryInfo = _workersSalary.FirstOrDefault(w => w.WorkerId == id);
                if (thisWorkerSalaryInfo != null)
                    mySubsSalary += thisWorkerSalaryInfo.WorkerSalary;
                else
                    Console.WriteLine(Resources.SalaryForManagerSalaryForSubNotFound);
            }

            var datesYearsDiff = GetYearsDifferenceInDates(fromDate, toDate);

            var myPositionData = AppMain.Instance.Positions.FirstOrDefault(p => p.Name == "Manager");
            if (myPositionData != null)
            {
                var basicSalary = myPositionData.BaseRate;
                var perYearRate = myPositionData.PerYearRate;
                var maxRate = myPositionData.MaxRate;
                var subsRate = myPositionData.SubsRate;

                var currentRate = perYearRate * datesYearsDiff + 0.0;

                if (currentRate > maxRate) currentRate = maxRate;

                result = basicSalary + (basicSalary / 100.0) * currentRate + (mySubsSalary / 100.0) * subsRate;
            }
            else
            {
                Console.WriteLine(Resources.ErrorManagerPositionNotFound);
            }

            return result;
        }

        // На вход приходит тот сотрудник, у которого в подчинении есть только сотрудники с просчитанной ЗП
        private static double CalcSalaryForSalesman(int userId, DateTime fromDate, DateTime toDate)
        {
            var result = 0.0;

            var mySubsSalary = 0.0;

            // Нужно получить идентификаторы для подчиненных только первого уровня
            var ids = UsersRepo.GetAllMySubWorkers(userId);
            foreach (var id in ids)
            {
                var thisWorkerSalaryInfo = _workersSalary.FirstOrDefault(w => w.WorkerId == id);
                if (thisWorkerSalaryInfo != null)
                    mySubsSalary += thisWorkerSalaryInfo.WorkerSalary;
                else
                    Console.WriteLine(Resources.SalaryForSalesmanSalaryForSubNotFound);
            }

            var datesYearsDiff = GetYearsDifferenceInDates(fromDate, toDate);

            var myPositionData = AppMain.Instance.Positions.FirstOrDefault(p => p.Name == "Salesman");
            if (myPositionData != null)
            {
                var basicSalary = myPositionData.BaseRate;
                var perYearRate = myPositionData.PerYearRate;
                var maxRate = myPositionData.MaxRate;
                var subsRate = myPositionData.SubsRate;

                var currentRate = perYearRate * datesYearsDiff + 0.0;

                if (currentRate > maxRate) currentRate = maxRate;

                result = basicSalary + (basicSalary / 100.0) * currentRate + (mySubsSalary / 100.0) * subsRate;
            }
            else
            {
                Console.WriteLine(Resources.ErrorSalesmanPositionNotFound);
            }

            return result;
        }

        // Считаем ЗП для сотрудников без подчиненных
        private static void CalcSalaryForWorkersWithoutSubWorkers(DateTime fromDate, DateTime toDate)
        {
            var users = UsersRepo.GetUsersWithoutSubWorkers();
            var positions = AppMain.Instance.Positions;

            foreach (var u in users)
            {
                double userResultSalary;
                var userPosition = positions.FirstOrDefault(p => p.Id == u.PositionId);
                if (userPosition != null)
                {
                    userResultSalary = MakeSalaryCalcForPosition(u.Id, userPosition, fromDate, toDate);
                }
                else
                {
                    // Посчитаем сотруднику без должности ЗП по базовой должности
                    userResultSalary = CalcSalaryForEmployee(fromDate, toDate);
                    Console.WriteLine(Resources.FoundWorkerWithoutRoleWithoutSubs);
                }

                // Добавляем ЗП в общий массив
                _workersSalary.Add(new SalaryOfWorker { WorkerId = u.Id, WorkerSalary = userResultSalary });
            }
        }

        // Получить идентификаторы известных ЗП работников
        private static List<int> GetKnownWorkersSalaryIds()
        {
            var resultList = new List<int>();
            foreach (var workerSalary in _workersSalary)
                resultList.Add(workerSalary.WorkerId);

            return resultList;
        }

        // Расчет ЗП для выбранной должности
        private static double MakeSalaryCalcForPosition(int userId, Position p, DateTime fromDate, DateTime toDate)
        {
            double userResultSalary;

            switch (p.Name)
            {
                case "Employee":
                    userResultSalary = CalcSalaryForEmployee(fromDate, toDate);
                    break;

                case "Manager":
                    userResultSalary = CalcSalaryForManager(userId, fromDate, toDate);
                    break;

                case "Salesman":
                    userResultSalary = CalcSalaryForSalesman(userId, fromDate, toDate);
                    break;

                // Посчитаем сотруднику без должности ЗП по базовой должности
                default:
                    Console.WriteLine(Resources.ErrorCalcSalaryNoRoleWorker);
                    userResultSalary = CalcSalaryForEmployee(fromDate, toDate);
                    break;
            }

            return userResultSalary;
        }

        // Функция для циклического подсчета ЗП сотрудников, на основе известных ЗП подчиненных сотрудников
        private static void CalcSalaryForWorkersWithKnownSubs(DateTime fromDate, DateTime toDate)
        {
            var workersIdsWithSalary = GetKnownWorkersSalaryIds();

            var users = UsersRepo.GetBossUsersWithSubsFromThisArr(workersIdsWithSalary);
            var positions = AppMain.Instance.Positions;

            foreach (var u in users)
            {
                double userResultSalary;
                var userPosition = positions.FirstOrDefault(p => p.Id == u.PositionId);
                if (userPosition != null)
                {
                    userResultSalary = MakeSalaryCalcForPosition(u.Id, userPosition, fromDate, toDate);
                }
                else
                {
                    // Посчитаем сотруднику без должности ЗП по базовой должности
                    userResultSalary = CalcSalaryForEmployee(fromDate, toDate);
                    Console.WriteLine(Resources.WorkerWithoutRoleWithKnownSubs);
                }

                // Добавляем ЗП в общий массив
                _workersSalary.Add(new SalaryOfWorker {WorkerId = u.Id, WorkerSalary = userResultSalary});
            }
        }
    }
}
