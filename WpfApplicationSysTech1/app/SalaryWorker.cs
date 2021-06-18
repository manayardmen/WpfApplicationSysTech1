using System;
using System.Collections.Generic;
using System.Linq;
using WpfApplicationSysTech1.app.repo;
using WpfApplicationSysTech1.app.types;

namespace WpfApplicationSysTech1.app
{
    // Рассчет зарплат
    public static class SalaryWorker
    {
        private static List<SalaryOfWorker> _workersSalary;

        private static void CalcSalaryForAllWorkers(DateTime fromDate, DateTime toDate)
        {
            Console.WriteLine("CalcSalaryForAllWorkers");
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
                Console.WriteLine("ЗП для этого сотрудника просто нет (GetSalaryForThisWorker)");

            return result;
        }

        // Получить разницу в годах
        private static int GetYearsDifferenceInDates(DateTime a, DateTime b)
        {
            DateTime beginTime = new DateTime(1, 1, 1);

            TimeSpan span = b - a;
            int years = (beginTime + span).Year - 1;

            return years;
        }

        // Сотрудник без подчиненных
        private static double CalcSalaryForEmployee(int userId, DateTime fromDate, DateTime toDate)
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
                Console.WriteLine("Ошибка, позиция Employee не найдена в списке позиций");
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
                    Console.WriteLine("SalaryForManager, не найдена зарплата подчиненного");
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
                Console.WriteLine("Ошибка, позиция Manager не найдена в списке позиций");
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
                    Console.WriteLine("SalaryForSalesman, не найдена зарплата подчиненного");
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
                Console.WriteLine("Ошибка, позиция Salesman не найдена в списке позиций");
            }

            return result;
        }

        private static void CalcSalaryForWorkersWithoutSubWorkers(DateTime fromDate, DateTime toDate)
        {
            var users = UsersRepo.GetUsersWithoutSubWorkers();
            var positions = AppMain.Instance.Positions;

            foreach (var u in users)
            {
                var userResultSalary = 0.0;
                var userPosition = positions.FirstOrDefault(p => p.Id == u.PositionId);
                if (userPosition != null)
                {
                    switch (userPosition.Name)
                    {
                        case "Employee":
                            userResultSalary = CalcSalaryForEmployee(u.Id, fromDate, toDate);
                            break;

                        case "Manager":
                            userResultSalary = CalcSalaryForManager(u.Id, fromDate, toDate);
                            break;

                        case "Salesman":
                            userResultSalary = CalcSalaryForSalesman(u.Id, fromDate, toDate);
                            break;

                        // Посчитаем сотруднику без должности ЗП по базовой должности
                        default:
                            Console.WriteLine("Ошибка, расчет ЗП идет не по должности (CalcSalaryForWorkersWithoutSubWorkers)");
                            userResultSalary = CalcSalaryForEmployee(u.Id, fromDate, toDate);
                            break;
                    }
                }
                else
                {
                    // Посчитаем сотруднику без должности ЗП по базовой должности
                    userResultSalary = CalcSalaryForEmployee(u.Id, fromDate, toDate);
                    Console.WriteLine("Найден сотрудник без должности (CalcSalaryForWorkersWithoutSubWorkers)");
                }

                // Добавляем ЗП в общий массив
                _workersSalary.Add(new SalaryOfWorker { WorkerId = u.Id, WorkerSalary = userResultSalary});
            }
        }

        // Функция для циклического подсчета ЗП сотрудников, на основе известных ЗП подчиненных сотрудников
        private static void CalcSalaryForWorkersWithKnownSubs(DateTime fromDate, DateTime toDate)
        {
            var workersIdsWithSalary = new List<int>();
            foreach (var workerSalary in _workersSalary)
                workersIdsWithSalary.Add(workerSalary.WorkerId);

            var users = UsersRepo.GetBossUsersWithSubsFromThisArr(workersIdsWithSalary);
            var positions = AppMain.Instance.Positions;

            foreach (var u in users)
            {
                var userResultSalary = 0.0;
                var userPosition = positions.FirstOrDefault(p => p.Id == u.PositionId);
                if (userPosition != null)
                {
                    switch (userPosition.Name)
                    {
                        case "Employee":
                            userResultSalary = CalcSalaryForEmployee(u.Id, fromDate, toDate);
                            break;

                        case "Manager":
                            userResultSalary = CalcSalaryForManager(u.Id, fromDate, toDate);
                            break;

                        case "Salesman":
                            userResultSalary = CalcSalaryForSalesman(u.Id, fromDate, toDate);
                            break;

                        // Посчитаем сотруднику без должности ЗП по базовой должности
                        default:
                            Console.WriteLine("Ошибка, расчет ЗП идет не по должности (CalcSalaryForWorkersWithKnownSubs)");
                            userResultSalary = CalcSalaryForEmployee(u.Id, fromDate, toDate);
                            break;
                    }
                }
                else
                {
                    // Посчитаем сотруднику без должности ЗП по базовой должности
                    userResultSalary = CalcSalaryForEmployee(u.Id, fromDate, toDate);
                    Console.WriteLine("Найден сотрудник без должности (CalcSalaryForWorkersWithKnownSubs)");
                }

                // Добавляем ЗП в общий массив
                _workersSalary.Add(new SalaryOfWorker { WorkerId = u.Id, WorkerSalary = userResultSalary});
            }
        }
    }
}
