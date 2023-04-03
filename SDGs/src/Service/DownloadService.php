<?php

namespace App\Service;

use Cake\Log\Log;

class DownloadService
{
    public $arrContentType = [
        'pdf' => 'application/pdf',
        'png' => 'image/png',
    ];

    public $defaultContentType = 'application/octet-stream';

    /**
     * Download method
     *
     * @param string $filePath
     * @return void|bool
     */
    public function download(string $filePath): bool
    {
        try {
            if(file_exists($filePath) && is_file($filePath)) {
                $extension = pathinfo($filePath, PATHINFO_EXTENSION);
                $fileSize = filesize($filePath);
                $contentType = $this->defaultContentType;

                if (isset($this->arrContentType[$extension])) {
                    $contentType = $this->arrContentType[$extension];
                }

                header('Content-Disposition: attachment; filename="' . basename($filePath) . '"');
                header('Content-Description: File Transfer');
                header('Content-Transfer-Encoding: binary');
                header('Cache-Control: public, must-revalidate, max-age=0');
                header('Pragma: public');
                header('Last-Modified: ' . gmdate('D, d M Y H:i:s') . ' GMT');
                header('Content-Type: ' . $contentType);
                header('Content-Length: ' . $fileSize);

                $resource = fopen($filePath, 'r');

                /*
                 * Checks if the end-of-file has been reached for the file
                 * */
                while (!feof($resource)) {
                    /*
                     * Read the file with the specific length/byte
                     * */
                    echo fread($resource, $fileSize);

                    flush();
                    sleep(1);
                }

                fclose($resource);

                return true;
            } else {
                Log::write('error',
                    sprintf("\n----- ERROR: THE FILE【%s】does not exist! ----- \n", $filePath));

                return false;
            }
        } catch (\Throwable $th) {
            Log::write('error',
                sprintf("\n----- ERROR WHEN DOWNLOADING THE FILE【%s】----- \n %s",
                    $filePath,
                    $th->getMessage()
                )
            );
        }
    }
}
