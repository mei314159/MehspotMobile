using System;
using System.Collections.Generic;
using System.Linq;

namespace Mehspot.Core.DTO
{
    public class ValidationModelBase
    {
        public virtual int Id { get; set; }

        private Dictionary<string, string> _ValidationErrors;
        private Dictionary<string, string> _GeneralErrors;
        private Dictionary<string, string> _BusinessLogicErrors;
        private List<Exception> _unProcesedExceptions;

        public Dictionary<string, string> ValidationErrors
        {
            get { return _ValidationErrors ?? (_ValidationErrors = new Dictionary<string, string>()); }
            private set { _ValidationErrors = value; }
        }
        protected Dictionary<string, string> GeneralErrors
        {
            get { return _GeneralErrors ?? (_GeneralErrors = new Dictionary<string, string>()); }
            private set { _GeneralErrors = value; }
        }
        protected Dictionary<string, string> BusinessLogicErrors
        {
            get { return _BusinessLogicErrors ?? (_BusinessLogicErrors = new Dictionary<string, string>()); }
            private set { _BusinessLogicErrors = value; }
        }
        protected List<Exception> UnprocessedExceptions
        {
            get { return _unProcesedExceptions ?? (_unProcesedExceptions = new List<Exception>()); }
            private set { _unProcesedExceptions = value; }
        }

        public bool HasValidationErrors
        {
            get { return ValidationErrors.Count > 0; }
        }

        public bool HasGeneralErrors
        {
            get { return GeneralErrors.Count > 0; }
        }

        public bool HasBusinessLogicErrors
        {
            get { return BusinessLogicErrors.Count > 0; }
        }

        public bool HasErrors
        {
            get { return HasValidationErrors | HasGeneralErrors | HasBusinessLogicErrors | HasUnprocessedExceptions; }
        }

        public IEnumerable<KeyValuePair<string, string>> GetValidationErrors()
        {
            return ValidationErrors.Keys.Select(key => new KeyValuePair<string, string>(key, ValidationErrors[key]));
        }

        public int ValidationErrorCount
        {
            get
            {
                return ValidationErrors.Count;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> GetGeneralErrors()
        {
            return GeneralErrors.Keys.Select(key => new KeyValuePair<string, string>(key, GeneralErrors[key]));
        }

        public IEnumerable<KeyValuePair<string, string>> GetBusinessLogicErrors()
        {
            return BusinessLogicErrors.Keys.Select(key => new KeyValuePair<string, string>(key, BusinessLogicErrors[key]));
        }

        public void AddGeneralError(string key, string errorMessage)
        {
            GeneralErrors.Add(key, errorMessage);
        }

        public void AddValidationError(string key, string validationErrorMessage)
        {
            ValidationErrors.Add(key, validationErrorMessage);
        }

        public void AddBusinessLogicError(string key, string businessLogicMessage)
        {
            BusinessLogicErrors.Add(key, businessLogicMessage);
        }

        public void AddUnprocessedException(Exception exception)
        {
            UnprocessedExceptions.Add(exception);
        }

        public void FlushUnprocessedExceptions()
        {
            UnprocessedExceptions.Clear();
        }

        public List<Exception> GetListOfUnprocessedExceptions()
        {
            return UnprocessedExceptions;
        }

        public bool HasUnprocessedExceptions
        {
            get { return UnprocessedExceptions.Count > 0; }
        }
    }
}