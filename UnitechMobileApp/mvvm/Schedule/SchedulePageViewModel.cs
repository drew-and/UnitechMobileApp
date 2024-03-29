﻿using System;
using System.Collections.Generic;
using System.Text;
using UnitechMobileApp.Model;
using UnitechMobileApp.mvvm.General;
using UnitechMobileApp.mvvm.Schedule.Accordion;
using UnitechMobileApp.mvvm.Schedule.WeekPicker;
using UnitechMobileApp.ScheduleHelper;

namespace UnitechMobileApp.mvvm.Schedule
{
    public class SchedulePageViewModel : ViewModelBase
    {
        private List<AccordionViewModel> accordionViewModels;
        private SchedulePage page;

        private string headerText;
        public string HeaderText
        {
            get { return headerText; }
            set { SetProperty(ref headerText, value); }
        }

        private Week selectedWeek;
        public Week SelectedWeek
        {
            get { return selectedWeek; }
            set
            {
                SetProperty(ref selectedWeek, value);
                OnWeekSelected();
            }
        }

        public SchedulePageViewModel(SchedulePage page, List<AccordionViewModel> accordionVms)
        {
            this.page = page;
            accordionViewModels = accordionVms;
            SelectedWeek = Workspace.ActiveUser.GetSchedule().Week;
        }

        private void OnWeekSelected()
        {
            Load();
        }

        /// <summary>
        /// Метод получает данные о текущем расписании и загружает их в интерфейс
        /// </summary>
        public void Load()
        {
            // Получаем распиание с портала на выбранную неделю
            ScheduleData schedule = Workspace.ActiveUser.GetSchedule(SelectedWeek);

            // Загрузка параметров
            HeaderText = schedule.GetHeaderText();

            // Загрузка расписания в аккордионы
            var dayLessonsPairs = schedule.DayLessonsPairs;
            foreach (KeyValuePair<int, List<ScheduleLesson>> day in dayLessonsPairs)
            {
                List<ScheduleDisplayLesson> displayLessons = new List<ScheduleDisplayLesson>();
                foreach (ScheduleLesson lesson in day.Value)
                {
                    displayLessons.Add(new ScheduleDisplayLesson(lesson));
                }
                accordionViewModels[day.Key - 1].Lessons = displayLessons;
            }
        }

        public void OpenWeekPickerPage()
        {
            page.Navigation.PushModalAsync(new WeekPickerPage(this));
        }
    }
}
