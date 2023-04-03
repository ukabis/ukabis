<?php

namespace App\Service;

use Cake\Log\Log;
use Cake\View\ViewBuilder;
use Mpdf\HTMLParserMode;
use Mpdf\Mpdf;

class PDFService extends Mpdf
{
    public function __construct(array $config = [])
    {
        $config += [
            'tempDir' =>  TMP . 'mpdf'
        ];
        parent::__construct($config);
    }

    /**
     * Convert HTML document to PDF
     *
     * @param array $data
     * @return string|null
     */
    public function convertHtmlToPdf(array $data): string|null
    {
        $view = new ViewBuilder();
        $view->disableAutoLayout();
        $view->setVars($data);
        $html = $view->build()->render('/certification/index');

        $styleSheet = file_get_contents(WWW_ROOT . CSS_CERTIFICATION_PATH);
        $this->useSubstitutions=true;
        $this->autoScriptToLang = true;
        $this->SetDisplayMode('fullpage');
        $this->SetDefaultBodyCSS('background', sprintf("url('%s')",WWW_ROOT . BACKGROUND_CERTIFICATION_PDF));
        $this->SetDefaultBodyCSS('background-image-resize', 6);

        $document = $this->purify_utf8($html);
        $filePath = $this->getFilePath($data);

        try {
            $this->WriteHTML($styleSheet, HTMLParserMode::HEADER_CSS);
            $this->WriteHTML($document, HTMLParserMode::HTML_BODY);
            $this->OutputFile(WWW_ROOT . $filePath);
        } catch (\Throwable $th) {
            Log::write('error',
                sprintf("\n----- ERROR WHEN WRITING FROM HTML TO PDF FILE ----- \n %s",
                    $th->getMessage()
                )
            );
        }

        if (!file_exists(WWW_ROOT . $filePath) || !filemtime(WWW_ROOT . $filePath)) {
            return null;
        }

        return $filePath;
    }

    /**
     * Generate File path
     *
     * @param array $data
     * @return string
     */
    public function getFilePath(array $data): string
    {
        $folder = '';
        switch ($data['office_type']) {
            case OFFICE_RESTAURANT:
                $folder = RESTAURANT_CERTIFICATION_FILE_PATH;
                break;

            case OFFICE_PRODUCER:
                $folder = PRODUCER_CERTIFICATION_FILE_PATH;
                break;
        }

        if (!is_dir(WWW_ROOT . BASE_CERTIFICATION_FOLDER)) {
            mkdir( WWW_ROOT . BASE_CERTIFICATION_FOLDER, 0775);
        }

        if (!is_dir(WWW_ROOT . $folder)) {
            mkdir(WWW_ROOT . $folder, 0775);
        }

        return sprintf("%s%s_certificate_%s.pdf",
            $folder,
            str_replace(' ', '_', $data['office_name']),
            date("YmdHis")
        );
    }
}
