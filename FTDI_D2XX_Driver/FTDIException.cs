/**
 * Copyright [2017] [Chen Zhu, zhuchen115@gmail.com]
 *
 *Licensed under the Apache License, Version 2.0 (the "License");
 *you may not use this file except in compliance with the License.
 *You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0

 *Unless required by applicable law or agreed to in writing, software
 *distributed under the License is distributed on an "AS IS" BASIS,
 *WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *See the License for the specific language governing permissions and
 *limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTDevice
{
    [Serializable]
    public class FTDIException :Exception
    {
        public FTDIException(FTStatus status) : base("FTDI Driver Returned Status:"+status+"Type:"+Enum.GetName(typeof(FTStatus), status))
        {
            
        }
        public FTDIException() : base()
        {

        }
        public FTDIException(string message) : base(message)
        {

        }
        public FTDIException(FTStatus status, string message) : base("FTDI Driver Returned Status:" + status + "Type:" + Enum.GetName(typeof(FTStatus), status)+" Message: "+message)
        {

        }

        public FTDIException(FTStatus status, string message, Exception inner) : base("FTDI Driver Returned Status:" + status + "Type:" + Enum.GetName(typeof(FTStatus), status) + " Message: " + message,inner)
        {

        }

        protected FTDIException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext contex)
        {
        }
    }
}
