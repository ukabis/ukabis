<?php

namespace App\Service;

use App\Controller\AppController;
use Cake\Database\Query;
use Cake\Mailer\MailerAwareTrait;

class SendMailService
{
    use MailerAwareTrait;

    /**
     * @var object
     */
    private object $params;

    public function __construct()
    {
        $this->params = new \stdClass();
    }

    /**
     * Email verification
     *
     * @param object $args
     * @return void
     */
    public function verify(object $args): void
    {
        $this->params->template = 'active_account';
        $this->params->subject = MAIL_SUBJECT_VERIFY;
        $this->params->destination_email = $args->email;

        $this->getMailer('App')->send('sendMail', [$this->params]);
    }

    /**
     * Email inquiry
     *
     * @param int $officeId
     * @return void
     */
    public function inquiry(AppController $controller, int $officeId): void
    {
        $authUser = $controller->Authentication->getIdentity()->getOriginalData();
        $fromOffice = $controller->Users->findById($authUser->id)
            ->contain([
                'Offices.Businesses' => [
                    'fields' => [
                        'Offices.name',
                        'Businesses.email'
                    ]
                ]
            ])
            ->first();

        $destinationOffice = $controller->Offices->findById($officeId)
            ->contain(['Businesses' => function (Query $query) {
                return $query->select(['name', 'email']);
            }])
            ->first();

        $this->params->from_office_name = $fromOffice->office->name;
        $this->params->from_email = $fromOffice->office->business->email;
        $this->params->destination_office_name = $destinationOffice->name;
        $this->params->destination_email = $destinationOffice->business->email;
        $this->params->template = 'inquiry';
        $this->params->subject = MAIL_SUBJECT_INQUIRY;

        $this->getMailer('App')->send('sendMail', [$this->params]);
    }

    /**
     * Email notify for certification
     *
     * @param object $data
     * @return void
     */
    public function certificationNotify(object $data): void
    {
        $this->params->business_name = $data->office->business->name;
        $this->params->destination_email = $data->office->business->email;
        $this->params->total_score = $data->total_score;
        $this->params->certificate_rank_label = $data->certificate_rank_label;
        $this->params->subject = MAIL_SUBJECT_SCORE_APPLICATION;
        $this->params->template = 'certification/deny';

        if (in_array($data->certificate_rank, APPROVE_RANK_LIST)) {
            $this->params->template = 'certification/approve';
            $this->params->certificate_path = $data->certificate_path;
            $this->params->mark_path =  $data->mark_path;
            $this->params->attachments = [
                WWW_ROOT . $data->certificate_path,
                WWW_ROOT . $data->mark_path,
            ];
        }

        $this->getMailer('App')->send('sendMail', [$this->params]);
    }
}
