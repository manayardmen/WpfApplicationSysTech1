using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfApplicationSysTech1.app.models;

namespace WpfApplicationSysTech1.app.repo
{
    // Операции над таблицей Users
    public static class UsersRepo
    {
        // Добавление нового пользователя
        public static bool AddNewUser(string userName, string userLogin, string userPassword)
        {
            var context = AppMain.Instance.Context;

            var existingUser = FindUser(userLogin);

            // Не нашли уже существующего пользователя
            if (existingUser == null)
            {
                var basicPositionId = PositionsRepo.GetBasicPositionId();
                var user = new User
                {
                    Name = userName,
                    Login = userLogin,
                    Password = userPassword,
                    IncomeDate = DateTime.Now.ToString("MM/dd/yyyy"),
                    PositionId = basicPositionId,
                    IsActive = 1
                };

                try
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, Const.AppName);
                    Console.WriteLine(e.Message);
                    existingUser = new User();
                }

            }

            return existingUser == null;
        }

        // Найти пользователя по id
        public static User FindUser(int userId)
        {
            var context = AppMain.Instance.Context;
            User result;

            try
            {
                result = context.Users.FirstOrDefault(u => u.Id == userId);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
                result = null;
            }

            return result;
        }

        // Найти пользователя по логину
        public static User FindUser(string userLogin)
        {
            var context = AppMain.Instance.Context;
            User result;

            try
            {
                result = context.Users.FirstOrDefault(u => u.Login == userLogin);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
                result = null;
            }

            return result;
        }

        // Получить пользователя по логину и паролю
        public static User GetUserByCredentials(string login, string password)
        {
            var context = AppMain.Instance.Context;
            User result;

            try
            {
                result = context.Users.FirstOrDefault(u => u.Login == login && u.Password == password);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
                result = null;
            }

            return result;
        }

        // Получить список пользователей
        public static List<User> GetUsersList()
        {
            var context = AppMain.Instance.Context;
            var result = context.Users.ToList();
            return result;
        }

        // Изменить имя пользователя найденного по id
        public static void SetNewNameToUser(int userId, string newName)
        {
            var context = AppMain.Instance.Context;

            try
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Name = newName;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
            }
        }

        // Изменить логин пользователя найденного по id
        public static void SetNewLoginToUser(int userId, string newLogin)
        {
            var context = AppMain.Instance.Context;

            try
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Login = newLogin;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
            }
        }

        // Изменить пароль пользователя найденного по id
        public static void SetNewPasswordToUser(int userId, string newPassword)
        {
            var context = AppMain.Instance.Context;

            try
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.Password = newPassword;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
            }
        }

        // Изменить роль пользователю
        public static void ChangeUserRole(int userId, int roleId)
        {
            var context = AppMain.Instance.Context;

            try
            {
                var user = context.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var positions = context.Positions.ToList();
                    var newUserPosition = positions.FirstOrDefault(p => p.Id == roleId);

                    if (newUserPosition != null)
                    {
                        if (newUserPosition.IsCanHaveSubs == 0)
                        {
                            // Теперь сотрудник не может иметь подчиненных
                            var isSuccess = RemoveThisUserIdAsBoss(userId);

                            if (isSuccess)
                            {
                                user.PositionId = roleId;
                                context.SaveChanges();
                            }
                            else
                            {
                                Console.WriteLine("Не удалось установить новую роль сотруднику, так как не удалось удалить его как начальника у других пользователей");
                                MessageBox.Show("Не удалось установить новую роль сотруднику, так как не удалось удалить его как начальника у других пользователей", Const.AppName);
                            }
                        }
                        else
                        {
                            user.PositionId = roleId;
                            context.SaveChanges();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Не удалось установить новую позицию для пользователя (позиция не найдена)");
                        MessageBox.Show("Не удалось установить новую позицию для пользователя (позиция не найдена)", Const.AppName);
                    }

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Const.AppName);
                Console.WriteLine(e.Message);
            }
        }

        // Удалить этого пользователя как начальника у других пользователей
        public static bool RemoveThisUserIdAsBoss(int userId)
        {
            var result = false;

            var context = AppMain.Instance.Context;

            try
            {
                var users = context.Users.Where(u => u.BossId == userId).ToList();
                foreach (var user in users)
                    user.BossId = -1;

                context.SaveChanges();

                result = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }

            return result;
        }

        // Получить список подчиненных сотрудников
        public static List<User> GetSubWorkersUsers(int userId)
        {
            var resultList = new List<User>();

            var context = AppMain.Instance.Context;

            try
            {
                resultList = context.Users.Where(u => u.BossId == userId).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }

            return resultList;
        }

        // Получить список свободных сотрудников
        public static List<User> GetFreeWorkersUsers(int thisUserId, int myBossId)
        {
            var resultList = new List<User>();

            var context = AppMain.Instance.Context;

            try
            {
                resultList = context.Users.Where(u => u.BossId <= 0 && u.Id != thisUserId && u.Id != myBossId).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }

            return resultList;
        }

        // Установить определенному пользователю, определенного начальника
        public static void SetToThisUserThisBoss(int targetUserId, int bossId)
        {
            var context = AppMain.Instance.Context;

            try
            {
                var targetUser = context.Users.FirstOrDefault(u => u.Id == targetUserId);

                if (targetUser != null)
                {
                    targetUser.BossId = bossId;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }
        }

        // Удалить у этого сотрудника начальника
        public static void ResetToThisUserThisBoss(int targetUserId)
        {
            SetToThisUserThisBoss(targetUserId, -1);
        }

        // Установить флаг активности для пользователя
        public static void SetActiveFlagToThisUser(int targetUserId, int activeFlag)
        {
            var context = AppMain.Instance.Context;

            try
            {
                var targetUser = context.Users.FirstOrDefault(u => u.Id == targetUserId);

                if (targetUser != null)
                {
                    targetUser.IsActive = activeFlag;
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }
        }

        // Удалить пользователя
        public static void RemoveThisUser(int targetUserId)
        {
            var context = AppMain.Instance.Context;

            try
            {
                User targetUser = context.Users.FirstOrDefault(u => u.Id == targetUserId);

                if (targetUser != null)
                {
                    var users = context.Users.Where(u => u.BossId == targetUser.Id).ToList();
                    foreach (var user in users)
                        user.BossId = -1;

                    context.Users.Remove(targetUser);
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Ошибка нет пользователя на удаление");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }
        }

        // Получить сотрудников, у которых в подчинении, только работники из этого массива
        public static List<User> GetBossUsersWithSubsFromThisArr(List<int> workersIds)
        {
            var context = AppMain.Instance.Context;

            var resultList = new List<User>();

            var currentUsers = context.Users.ToList();
            foreach (var bossUser in currentUsers)
            {
                var subWorkers =  context.Users.Where(u => u.BossId == bossUser.Id).ToList();
                var isOnlyKnownWorkers = false;
                foreach (var subWorker in subWorkers)
                {
                    if (!workersIds.Exists(x=> x == subWorker.Id))
                    {
                        isOnlyKnownWorkers = false;
                        break;
                    }

                    isOnlyKnownWorkers = true;
                }

                if (isOnlyKnownWorkers)
                    resultList.Add(bossUser);
            }

            return resultList;
        }

        // Получить список пользователей без подчиненных
        public static List<User> GetUsersWithoutSubWorkers()
        {
            var context = AppMain.Instance.Context;

            var resultList = new List<User>();

            var currentUsers = context.Users.ToList();
            foreach (var user in currentUsers)
            {
                var anySubWorker = context.Users.FirstOrDefault(u => u.BossId == user.Id);
                if (anySubWorker == null)
                {
                    resultList.Add(user);
                }
            }

            return resultList;
        }

        // Получить идентификаторы для подчиненных только первого уровня
        public static List<int> GetOnlyMySubWorkers(int userId)
        {
            var context = AppMain.Instance.Context;
            var resultList = new List<int>();

            try
            {
                var preResultList = context.Users.Where(w => w.BossId == userId).ToList();
                foreach (var w in preResultList)
                    resultList.Add(w.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                MessageBox.Show(e.Message, Const.AppName);
            }

            Console.WriteLine("Подчиненных для кого-то (GetOnlyMySubWorkers):");
            Console.WriteLine(resultList.Count);

            return resultList;
        }

        // Получить идентификаторы для подчиненных со всех уровней
        public static List<int> GetAllMySubWorkers(int userId)
        {
            var resultList = new List<int>();

            var fistLevelSubs = GetOnlyMySubWorkers(userId);

            var nextLevelSubs = new List<int>();

            foreach (var s in fistLevelSubs)
                nextLevelSubs.Add(s);

            do
            {
                // Собираем с первого уровня (пердыдущего) в результат
                foreach (var subId in fistLevelSubs)
                    resultList.Add(subId);

                // Сначала очищаем следующий уровень
                nextLevelSubs.Clear();

                // Пробегаемся по первому уровню и собираем их всех в следующий уровень
                foreach (var subId in fistLevelSubs)
                {
                    var subs = GetOnlyMySubWorkers(subId);

                    foreach (var s in subs)
                        nextLevelSubs.Add(s);
                }

                // Обработали предыдущий уровень, очищаем
                fistLevelSubs.Clear();

                // Собираем с нового уровня в предыдущий
                foreach (var subId in nextLevelSubs)
                    fistLevelSubs.Add(subId);

            } while (nextLevelSubs.Count > 0);

            Console.WriteLine("Подчиненных для Salesman:");
            Console.WriteLine(resultList.Count);

            return resultList;
        }
    }
}
