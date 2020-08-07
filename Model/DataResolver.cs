using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;

namespace PdfPlayGround.Model
{
    class DataResolver : IDisposable
    {
		private readonly GraphQLHttpClient _client;

		public DataResolver()
        {
			string endpoint = "https://api-sit.endataclaims.com/midgard/graphql";
			_client = new GraphQLHttpClient(endpoint, new NewtonsoftJsonSerializer());

			BuildToken();
		}

		public async Task<ClaimJobReportForm> GetFormData(string claimId)
		{
			var request = new GraphQLRequest
			{
				Query = _queryFormData,
				OperationName = "ReportQuery",
				Variables = new
                {
					claimId
				}
            };
			var response = await _client.SendQueryAsync<DataResolveType>(request);
			var claimJob = response.Data.ClaimJob;
			claimJob.FillData();
			return claimJob;
		}

		private void BuildToken()
        {
			var tokenRequest = new GraphQLRequest
			{
				Query = _queryToken
			};
			var responseToken = _client.SendQueryAsync<dynamic>(tokenRequest).Result;
			var token = responseToken.Data.userAuthenticateAllServer[0]["accessToken"].Value;
			_client.HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

		}

		#region Dispose Code
		private bool disposed = false;

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed) return;
			if (disposing) { }
			_client.Dispose();
			disposed = true;
		}

		~DataResolver() { Dispose(false); }
		#endregion

		#region Query

		private readonly string _queryToken = @"
mutation{
  userAuthenticateAllServer(input:{
    userLogin: ""gbendata1""
    password: ""gbendata1""
  }){
    accessToken
  }
}";

        private readonly string _queryFormData = @"
query ReportQuery($claimId: ID!) {
	claimJob(where: { id: $claimId }) {
		id
		reportFormId
		refNumber
		insurer{
		  companyName
		  companyPhone1
		  companyPhone2
		  companyEmail
      companyAddress{
        line1
			  line2
			  suburb
			  state
			  postcode
      }
		}
		insured{
		  name
		  email
      postalAddress{
        line1
			  line2
			  suburb
			  state
			  postcode
      }
		}
		caseManager{
		  managerName
		}
    incidentDetail{
      riskAddress{
        line1
			  line2
			  suburb
			  state
			  postcode
      }
    }
		building{
		  scopingSupplier{
        companyName
        companyPhone1
        logoright
        companyPhone2
        companyId
        companyAddress{
          line1
          line2
          suburb
          state
          postcode
        }
		  }
		  authorisedSupplier{
			companyName
			companyPhone1
			companyPhone2
        companyAddress{
          line1
          line2
          suburb
          state
          postcode
        }
		  }
		}
		reportForm {
			cards {
				...ReportCard_cart
			}
		}
		reportData {
			data
		}
	}
}

fragment ReportCard_cart on Card {
	title
	fields {
		fieldType:__typename
    label
		...ReportCardField_field
	}
}

fragment ReportCardField_field on Field {
	...ReportInfoField_field
	...ReportTextField_field
	...ReportTextAreaField_field
	...ReportSelectField_field
	...ReportDateTimeField_field
	...ReportGroupField_field
	...ReportFileField_field
}
fragment ReportDateTimeField_field on DateTimeField {
	label
	name
}
fragment ReportFileField_field on FileField {
	label
	name
}
fragment ReportInfoField_field on InfoField {
	label
	value
}
fragment ReportTextAreaField_field on TextAreaField {
	label
	name
}
fragment ReportTextField_field on TextField {
	label
	name
}
fragment ReportGroupField_field on GroupField {
	label
	name
	...ArrayField_data
}
fragment ReportSelectField_field on SelectField {
	name
	label
	options {
		label
		value
	}
}

fragment ArrayField_data on GroupField {
	label
	name
	fields {
    __typename
		fieldType:__typename
		label
		...ReportInfoField_field
		...ReportTextField_field
		...ReportTextAreaField_field
		...ReportSelectField_field
		...ReportDateTimeField_field
		...ReportFileField_field
	}
}";
        #endregion
    }

	internal class DataResolveType
    {
		public ClaimJobReportForm ClaimJob { get; set; }
    }
}
