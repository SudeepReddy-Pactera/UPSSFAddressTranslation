using System;
using System.Collections.Generic;
using System.Text;
using UPS.DataObjects.Reports;
using UPS.DataObjects.UserData;
using UPS.DataObjects.WR_FLW;

namespace UPS.ServicesAsyncActions
{
    public interface IWorkflowAsync
    {
        WorkflowDataResponse SelectWorkflows(User user);
        WorkflowDataResponse InsertWorkflow(WorkflowDataRequest workflowData);

        List<SummaryData> getSummaryData(DateTime fromDate, DateTime toDate);

        //WorkflowDataResponse GetWorkflowData(int shipid);
        //List<WorkflowDataResponse> GetWorkflowDataList(int workid);

    }
}
