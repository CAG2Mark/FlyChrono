/*
    Copyright (C) 2024 Mark Ng

    This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation; either version 3 of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along with this program; if not, see <https://www.gnu.org/licenses>.

    Additional permission under GNU GPL version 3 section 7

    If you modify this Program, or any covered work, by linking or combining it with "FSUIPC Client DLL for .NET" (or a modified version of that library), containing parts covered by the terms of its license (available at http://fsuipc.paulhenty.com/), the licensors of this Program grant you additional permission to convey the resulting work.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FlyChrono2.Views
{
    /// <summary>
    /// Interaction logic for License.xaml
    /// </summary>
    public partial class License : Window
    {
        public License()
        {
            InitializeComponent();

            var stream = new MemoryStream(ASCIIEncoding.Default.GetBytes("{\\rtf1\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Segoe UI;}}\r\n{\\*\\generator Riched20 10.0.17134}\\viewkind4\\uc1 \r\n\\pard\\sa200\\sl276\\slmult1\\b\\f0\\fs40\\lang9 FLYAPPS END USER LICENSE AGREEMENT\\b0\\fs22\\par\r\nAll FlyApps applications are owned by FlyApps. By using these applications, you agree to be bound by a legal contract of which is displayed below.\\par\r\n\\par\r\n\\b 1) COPYRIGHT\\b0\\par\r\nAll images, text and files contained within this package (ie. content in the install package you downloaded, content contained within the install directory, content contained within the application itself) not owned by a third party are intellectual property of FlyApps. The images, text and files are protected by copyright and other intellectual property laws.\\par\r\nBy installing and using software from FlyApps, you hereby agree you shall not copy, distribute, publish, modify or create derivative works from the content mentioned above, with the exception of overlay templates, of which you are free to use, but not publish. Additionally, you agree the content mentioned above shall not be utilized against FlyApps or be involved in any illegal activity. Same may not be used utilized in any commercial context, corporate entity or similar.\\par\r\n\\par\r\n\\b 2) LICENSE\\b0\\par\r\nAll FlyApps applications are freeware. It may be freely distributed given that the distributor does not: claim the software as their own, charge money for the software, impose further restrictions on the usage of the software, or does not distribute the file retaining this license. The utilization of the software shall not be in the amount of commercial use OTHER than revenue from video sharing/streaming services when used appropriately, and/or when used to accompany such activities. Should the user violate any of the above terms, the individual or group in question shall face legal charges. FlyApps reserves the right to have the final say in all appropriate circumstances.\\par\r\n\\par\r\n\\b 3) WARRANTY, LIMTATIONS & DISCLAIMER\\b0\\par\r\nTHIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS \"AS IS\" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.\\par\r\n\\u8203?Installation and usage of the applications is completely at your own risk. \\u8203?In circumstances, whereat damage, loss or charges are the indirect or direct consequence of your use of the software, the author shall not be liable for it.\\par\r\n}\r\n"));
            LicenseRichTextBox.Selection.Load(stream, DataFormats.Rtf);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
