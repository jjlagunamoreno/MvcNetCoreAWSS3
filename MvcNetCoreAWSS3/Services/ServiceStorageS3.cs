using Amazon.S3.Model;
using Amazon.S3;
using System.Net;

public class ServiceStorageS3
{
    //VAMOS A RECIBIR EL NOMBRE DEL BUCKET
    //A PARTIR DEL APPSETTINGS.JSON
    private string BucketName;

    //LA CLASE/INTERFACE PARA TRABAJAR CON LOS BUCKETS
    //SE LLAMA IAmazonS3 Y VAMOS A RECIBIRLA MEDIANTE
    //INYECCION
    private IAmazonS3 ClientS3;

    public ServiceStorageS3(IConfiguration configuration
        , IAmazonS3 clientS3)
    {
        this.BucketName = configuration.GetValue<string>
            ("AWS:BucketName");
        this.ClientS3 = clientS3;
    }

    //COMENZAMOS CREANDO UN METODO PARA SUBIR FICHEROS
    public async Task<bool> UploadFileAsync
        (string fileName, Stream stream)
    {
        PutObjectRequest request = new PutObjectRequest
        {
            Key = fileName,
            BucketName = this.BucketName,
            InputStream = stream
        };
        //PARA TRABAJAR SE UTILIZA LA CLASE IAmazonS3 
        //CON UNA PETICION DE PUTOBJECT
        PutObjectResponse response = await
            this.ClientS3.PutObjectAsync(request);
        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> DeleteFileAsync
        (string fileName)
    {
        DeleteObjectResponse response = await
            this.ClientS3.DeleteObjectAsync
            (this.BucketName, fileName);
        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //METODO PARA RECUPERAR TODOS LOS FICHEROS DEL BUCKET
    //AUNQUE NO TENGAMOS VERSIONES, DEBEMOS INDICAR LAS VERSIONES
    //EN EL RECORRIDO DE LOS FICHEROS.
    public async Task<List<string>> GetVersionsFileAsync()
    {
        ListVersionsResponse response = await
            this.ClientS3.ListVersionsAsync(this.BucketName);
        //EXTREAMOS TODAS LAS KEYS DE NUESTROS FICHEROS, ES DECIR, 
        //EL NOMBRE DE TODOS LOS FICHEROS.  POR DEFECTO NOS 
        //DEVUELVE LA ULTIMA VERSION
        List<string> fileNames =
            response.Versions.Select(x => x.Key).ToList();
        return fileNames;
    }
}
