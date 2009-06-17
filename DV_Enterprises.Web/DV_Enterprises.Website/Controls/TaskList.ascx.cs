﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DV_Enterprises.Web.Data.Domain;
using DV_Enterprises.Web.Data.Filters;
using DV_Enterprises.Web.Service;
using DV_Enterprises.Web.Service.Interface;

namespace Controls
{
    public partial class TaskList : UserControl
    {
        #region Static properties

        #endregion

        #region Instance properties

        private readonly static IWebContext WebContext = new WebContext();
        private readonly static IRedirector Redirector = new Redirector();
        private int _taskTypeID;
        private int _sectionID;
        private string _message;

        [Category("Appearance")]
        public int SectionID
        {
            get
            {
                if (_sectionID == 0 && litSectionID.Text != string.Empty)
                {
                    _sectionID = Convert.ToInt32(litSectionID.Text);
                }
                return _sectionID;
            }
            set
            {
                _sectionID = value;
                litSectionID.Text = value.ToString();
            }
        }
        [Category("Appearance")]
        public TaskTypes Type { get; set; }
        [Category("Appearance")]
        public string Message
        {
            get
            {
                if (_message == string.Empty && litTaskMessage.Text != string.Empty)
                {
                    _message = litTaskMessage.Text;
                }
                return _message;
            }
            set { _message = litTaskMessage.Text = value; }
        }

        public int TaskTypeID
        {
            get { return _taskTypeID != 0 ? _taskTypeID : (TaskTypeID = TaskType.Find().ByType(Type).ID); }
            private set { _taskTypeID = value; }
        }

        #endregion

        #region Static methods

        private static List<Task> Tasks(int sectionID, int taskTypeID)
        {
            return Task.Find().Where(t => t.SectionID == sectionID && t.TaskTypeId == taskTypeID).ToList();
        }

        #endregion

        #region Intance methods

        protected void Page_Load(object sender, EventArgs e)
        {
            HidePanel();
            if (IsPostBack) return;
            litTaskTitle.Text = string.Format("{0} tasks", Type.ToString().DeCamelize());
            Bind();
        }
        protected void btnAddTask_Click(object sender, EventArgs e)
        {
            ShowPanel();
        }
        protected void btnSaveTask_Click(object sender, EventArgs e)
        {
            var startHour = ddlStartTimeAmPm.SelectedValue.ToLower() == "am"
                                ? Convert.ToInt32(ddlStartTimeHours.SelectedValue)
                                : Convert.ToInt32(ddlStartTimeHours.SelectedValue) + 12;
            var startMin = Convert.ToInt32(ddlStartTimeMinutes.SelectedValue);
            var interval = Convert.ToDouble(ddlInterval.SelectedValue);

            new Task
                {
                    ID = 0,
                    SectionID = SectionID,
                    TaskTypeId = TaskTypeID,
                    StartTime =
                        new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour,
                                     startMin, 0),
                    Interval = interval
                }.Save();
            HidePanel();
            Redirector.GoToViewGreenhouse(WebContext.GreenhouseId);
        }
        protected void gvwTasks_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var startTime = Convert.ToDateTime(gvwTasks.Rows[e.RowIndex].Cells[0].Text);
            Task.Find().Where(t => t.StartTime == startTime && t.SectionID == SectionID && t.TaskTypeId == TaskTypeID).First().Delete();
            Bind();
            Redirector.GoToViewGreenhouse(WebContext.GreenhouseId);
        }

        private void Bind()
        {
            gvwTasks.DataSource = Tasks(SectionID, TaskTypeID);
            gvwTasks.DataBind();
        }
        private void ShowPanel()
        {
            pnlAddTask.Visible = true;
            btnAddTask.Visible = false;
        }
        private void HidePanel()
        {
            btnAddTask.Visible = true;
            pnlAddTask.Visible = false;
        }
        

        #endregion
    }
}