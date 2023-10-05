/************************************************************************
 *                                                                      *
 *                            CTask.cs                                  *
 *                                                                      *
 *      Kodlama:                                                        *
 *                                                                      *
 *          Burak Akat (Nexor)                                          *
 *                                                                      *
 *      Tarih:                                                          *
 *                                                                      *
 *          21.07.2023                                                  *
 *                                                                      *
 *      İşlev:                                                          *
 *                                                                      *
 *          Asekron olarak çalıştırılmış bir task görevini              *
 *          iptal etmek için hazırlanmış bir yapıdır.                   *
 *                                                                      *
 *          Dikkat edilmesi gereken şey, çalıştırılan fonksiyonun       *
 *          ASYNC olarak tanımlanmış olması gerekmekte ve try catch     *
 *          kullanılmalıdır. Try kısmının içine çalıştırılacak kodlar   *
 *          catch içi ise boş bırakılabilir, çünkü task işlemi iptal    *
 *          olunca catch kısmı devreye girecektir.                      *
 *                                                                      *
 *          Son olarak bekletme işlemlerinde Thread.Sleep değil         *
 *                                                                      *
 *          await Task.Delay(sure, token); şeklinde ayarlanması         *
 *          gerekmektedir.                                              *
 *                                                                      *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public class CTask
{
    private static CancellationTokenSource TokenSource;

    public static CancellationToken Token_Olustur()
    {
        TokenSource = new CancellationTokenSource();
        return TokenSource.Token;
    }

    public static CancellationToken Token_Al()
    {
        if(!TokenSource.IsCancellationRequested)
        {
            return TokenSource.Token;
        }
        return default;
    }

    public static void Durdur()
    {
        if(TokenSource != null)
        {
            CBrowser.TaskDurdur = true;
            TokenSource.Cancel();
        }
    }
}