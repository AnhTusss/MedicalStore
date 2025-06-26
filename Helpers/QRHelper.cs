using QRCoder;

namespace MedicalStore.Helpers
{
    public class QRHelper
    {
        public static string GenerateBankTransferQR(string bankCode, string accountNumber, string accountName, decimal amount, string note)
        {
            // VietQR định dạng theo https://vietqr.net
            string qrContent = $"bank:{bankCode}|account:{accountNumber}|amount:{amount}|note:{note}|name:{accountName}";

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(10);

            string base64Image = Convert.ToBase64String(qrCodeBytes);
            return $"data:image/png;base64,{base64Image}";
        }
    }
}
