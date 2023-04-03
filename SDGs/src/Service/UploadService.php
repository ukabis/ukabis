<?php
declare(strict_types=1);

namespace App\Service;

use Cake\Log\Log;
use Laminas\Diactoros\UploadedFile;

class UploadService
{
    /**
     * Upload file
     * @param UploadedFile $file
     * @param integer $officeType
     * @return array
     */
    public function upload(UploadedFile $file, int $officeType): array
    {
        $baseName = $file->getClientFilename();
        $fileName = pathinfo($baseName, PATHINFO_FILENAME);
        $fileExtension = pathinfo($baseName, PATHINFO_EXTENSION);
        $encryptFilename = $this->encryptFileName($fileExtension);
        $targetPath = $this->createTargetFilePath($officeType, $encryptFilename);
        $file->moveTo(WWW_ROOT. $targetPath);

        return [
            'file_original_name' => $fileName,
            'file_original_extension' => $fileExtension,
            'path' => $targetPath,
        ];
    }

    /**
     * Upload and convert image to PNG
     * @param UploadedFile $file
     * @param integer $officeType
     * @return array
     */
    public function uploadAndConvertToPNG(UploadedFile $file, int $officeType): array
    {
        $data = $this->upload($file, $officeType);

        if ($data['file_original_extension'] && $data['file_original_name'] && $data['path'] && $data['file_original_extension'] != 'png') {
            $encryptFilenamePNG = $this->encryptFileName('png');
            $targetPathPNG = $this->createTargetFilePath($officeType, $encryptFilenamePNG);
            $result = imagepng(imagecreatefromjpeg(WWW_ROOT. $data['path']), $targetPathPNG);

            if ($result) {
                if (file_exists(WWW_ROOT. $data['path'])) {
                    unlink(WWW_ROOT. $data['path']);
                }
                $data['path'] = $targetPathPNG;
                $data['file_current_extension'] = 'png';
            }
        }

        return $data;
    }

    /**
     * Create target file path to upload
     *
     * @param string $baseName File name with extension
     * @param integer $officeType
     * @return string
     */
    public function createTargetFilePath(int $officeType, string $baseName): string
    {
        $folderUpload = '';
        switch ($officeType) {
            case OFFICE_RESTAURANT:
                $folderUpload = RESTAURANT_PUBLIC_FILE_PATH;
                break;
            case OFFICE_PRODUCER:
                $folderUpload = PRODUCER_PUBLIC_FILE_PATH;
                break;
        }

        if (!is_dir(WWW_ROOT . $folderUpload)) {
            mkdir(WWW_ROOT . $folderUpload, 0775);
        }

        return sprintf("%s%s", $folderUpload, $baseName);
    }

    /**
     * Encrypt file name.
     *
     * @param string $extension
     *
     * @return string
     */
    public function encryptFileName(string $extension): string
    {
        return sprintf("%s_%s.%s", date("YmdHis"), mt_rand(100000, 999999), $extension);
    }

    /**
     * Delete file.
     *
     * @param string|null $path
     *
     * @return bool
     */
    public function destroyFile(?string $path): bool
    {
        if ($path && file_exists(WWW_ROOT. $path)) {
            unlink(WWW_ROOT. $path);

            return true;
        }

        return false;
    }
}
