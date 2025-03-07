//////////////////////////////// 
// 
//   Copyright 2023 Battelle Energy Alliance, LLC  
// 
// 
//////////////////////////////// 
using System.IO;
using System;
using System.Linq;
using CSETWebCore.DataLayer.Manual;
using CSETWebCore.DataLayer.Model;
using CSETWebCore.Interfaces.Helpers;
using CSETWebCore.Interfaces.IRP;
using CSETWebCore.Model.Acet;
using CSETWebCore.Business.Acet;
using Npoi.Mapper;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using CSETWebCore.Helpers;

namespace CSETWebCore.Business.IRP
{
    public class IRPBusiness : IIRPBusiness
    {
        private CSETContext _context;
        private readonly IAssessmentUtil _assessmentUtil;

        public IRPBusiness(CSETContext context, IAssessmentUtil assessmentUtil)
        {
            _context = context;
            _assessmentUtil = assessmentUtil;
        }
        public IRPResponse GetIRPList(int assessmentId, bool spanishFlag = false)
        {
            IRPResponse response = new IRPResponse();
            Dictionary<int, IRPModel> dictionary = new Dictionary<int, IRPModel>();
            Dictionary<int, IRPSpanishRow> dictionaryHeaders = new Dictionary<int, IRPSpanishRow>();
            if (spanishFlag) 
            {
                dictionary = AcetBusiness.buildIRPDictionary();
                dictionaryHeaders = AcetBusiness.buildIRPHeaderDictionary();
            }

            foreach (IRP_HEADER header in _context.IRP_HEADER)
            {
                IRPHeader tempHeader = new IRPHeader()
                {
                    header = header.Header
                };

                if (spanishFlag)
                {
                    var output = new IRPSpanishRow();
                    if (dictionaryHeaders.TryGetValue(header.IRP_Header_Id, out output))
                        tempHeader.header = dictionaryHeaders[header.IRP_Header_Id].SpanishHeader;
                }

                foreach (DataLayer.Model.IRP irp in _context.IRP.Where(x => x.Header_Id == header.IRP_Header_Id).ToList())
                {
                    IRPModel tempIRP = new IRPModel()
                    {
                        IRP_Id = irp.IRP_ID,
                        Item_Number = irp.Item_Number ?? 0,
                        Description = irp.Description,
                        DescriptionComment = irp.DescriptionComment,
                        Risk_1_Description = irp.Risk_1_Description,
                        Risk_2_Description = irp.Risk_2_Description,
                        Risk_3_Description = irp.Risk_3_Description,
                        Risk_4_Description = irp.Risk_4_Description,
                        Risk_5_Description = irp.Risk_5_Description,
                        Validation_Approach = irp.Validation_Approach,
                        Risk_Type = irp.Risk_Type
                    };

                    if (spanishFlag)
                    {
                        var output = new IRPModel();
                        var temp = new IRPModel();
                        if (dictionary.TryGetValue(tempIRP.IRP_Id, out output))
                        {
                            tempIRP.Description = dictionary[tempIRP.IRP_Id].Description;
                            tempIRP.Risk_1_Description = dictionary[tempIRP.IRP_Id].Risk_1_Description;
                            tempIRP.Risk_2_Description = dictionary[tempIRP.IRP_Id].Risk_2_Description;
                            tempIRP.Risk_3_Description = dictionary[tempIRP.IRP_Id].Risk_3_Description;
                            tempIRP.Risk_4_Description = dictionary[tempIRP.IRP_Id].Risk_4_Description;
                            tempIRP.Risk_5_Description = dictionary[tempIRP.IRP_Id].Risk_5_Description;
                            tempIRP.DescriptionComment = dictionary[tempIRP.IRP_Id].DescriptionComment;
                            tempIRP.Validation_Approach = dictionary[tempIRP.IRP_Id].Validation_Approach;
                        }
                    }

                    // Get the existing answer or create a blank 
                    ASSESSMENT_IRP answer = _context.ASSESSMENT_IRP.FirstOrDefault(ans =>
                        ans.IRP_Id == irp.IRP_ID &&
                        ans.Assessment.Assessment_Id == assessmentId);
                    if (answer == null)
                    {
                        answer = new ASSESSMENT_IRP()
                        {
                            Assessment_Id = assessmentId,
                            IRP_Id = irp.IRP_ID,
                            Response = 0,
                            Comment = ""
                        };

                        _context.ASSESSMENT_IRP.Add(answer);
                    }
                    tempIRP.Response = answer.Response.Value;
                    tempIRP.Comment = answer.Comment;
                    tempHeader.irpList.Add(tempIRP);
                }

                response.headerList.Add(tempHeader);
            }
            if (_context.SaveChanges() > 0)
            {
                _assessmentUtil.TouchAssessment(assessmentId);
            }

            
            return response;
        }

        public void PersistSelectedIRP(int assessmentId, IRPModel irp)
        {
            if (assessmentId == 0) { return; }
            if (irp == null) { return; }


            ASSESSMENT_IRP answer = _context.ASSESSMENT_IRP.FirstOrDefault(i => i.IRP_Id == irp.IRP_Id &&
                i.Assessment.Assessment_Id == assessmentId);
            if (answer != null)
            {
                answer.Response = irp.Response;
                answer.Comment = irp.Comment;
            }
            else
            {
                answer = new ASSESSMENT_IRP()
                {
                    Response = irp.Response,
                    Comment = irp.Comment,
                };
                answer.Assessment = _context.ASSESSMENTS.FirstOrDefault(a => a.Assessment_Id == assessmentId);
                _context.ASSESSMENT_IRP.Add(answer);
            }
            if (_context.SaveChanges() > 0)
            {
                _assessmentUtil.TouchAssessment(assessmentId);
            }
        }
    }
}