/************************************************************************
 *                                                                      *
 *                           CInvoke.cs                                 *
 *                                                                      *
 *      Kodlama:                                                        *
 *                                                                      *
 *          Burak Akat (Nexor)                                          *
 *                                                                      *
 *      Tarih:                                                          *
 *                                                                      *
 *          19.07.2023                                                  *
 *                                                                      *
 *      İşlev:                                                          *
 *                                                                      *
 *          İş parçacıklarında çalıştırılan bir görevde                 *
 *          Form elemanlarını düzenlerken çakışmaması için              *
 *          hazırlanmış bir yapıdır. Form elemanlarını güncellemek      *
 *          için kullanılır.                                            *
 *                                                                      *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CInvoke
{
    public static void Update_Label(System.Windows.Forms.Label label, string icerik, params object[] format)
    {
        if (label.InvokeRequired)
        {
            label.Invoke(new Action(() =>
            {
                label.Text = string.Format(icerik, format);
            }));
        }
        else
        {
            label.Text = string.Format(icerik, format);
        }
    }

    public static void Update_ProgressBar(System.Windows.Forms.ProgressBar bar, int deger)
    {
        if (bar.InvokeRequired)
        {
            bar.Invoke(new Action(() =>
            {
                bar.Value = deger;
            }));
        }
        else
        {
            bar.Value = deger;
        }
    }
}