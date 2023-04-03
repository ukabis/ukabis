<?php

namespace App\Mailer;

use Cake\Log\Log;
use Cake\Mailer\Mailer;

/**
 *
 * Customize AppMailer class extends from Mailer base class
 */
class AppMailer extends Mailer
{
    /**
     * Constructor
     */
    public function __construct($config = null)
    {
        parent::__construct($config);
        $this->setEmailFormat('html');
    }

    /**
     * Send mail method
     *
     * @param object $params
     * @return void
     */
    public function sendMail(object $params): void
    {
        /**
         * Send with attachments
         */
        if (isset($params->attachments)) {
            try {
                $this->setAttachments($params->attachments);
            } catch (\Throwable $th) {
                Log::write('error',
                    sprintf("\n----- ERROR WHEN SENDING EMAIL WITH FILE TO【%s】----- \n %s",
                        $params->destination_email,
                        $th->getMessage()
                    )
                );
            }
        }

        if (isset($params->from_email)) {
            $this->setSender($params->from_email);
        }

        $this->setTo($params->destination_email)
            ->setSubject($params->subject)
            ->setViewVars(['params' => $params])
            ->viewBuilder()
                ->setTemplate($params->template);
    }
}
