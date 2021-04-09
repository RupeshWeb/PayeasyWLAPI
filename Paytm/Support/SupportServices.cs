using DataModel.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessServices
{
    public class SupportServices : ISupportServices
    {
        private readonly UnitOfWork _unitOfWork;

        /// <summary>
        /// Public constructor.
        /// </summary>
        public SupportServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


    }
}
