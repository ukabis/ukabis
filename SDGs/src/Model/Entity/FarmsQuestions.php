<?php
declare(strict_types=1);

namespace App\Model\Entity;

use Cake\ORM\Entity;

/**
 * Farm Question Entity
 *
 * @property int $id
 * @property int $office_id
 * @property string|null $answer_questions
 * @property int|null $status
 * @property int|null $total_score
 * @property \Cake\I18n\FrozenTime $created_at
 * @property \Cake\I18n\FrozenTime $modified_at
 */
class FarmsQuestions extends Entity
{
    /**
     * Fields that can be mass assigned using newEntity() or patchEntity().
     *
     * Note that when '*' is set to true, this allows all unspecified fields to
     * be mass assigned. For security purposes, it is advised to set '*' to false
     * (or remove it), and explicitly make individual fields accessible as needed.
     *
     * @var array<string, bool>
     */
    protected $_accessible = [
        'office_id' => true,
        'answer_questions' => true,
        'status' => true,
        'question1_score' => true,
        'question2_score' => true,
        'question3_score' => true,
        'question4_score' => true,
        'question5_score' => true,
        'question6_score' => true,
        'question7_score' => true,
        'question8_score' => true,
        'question9_score' => true,
        'question10_score' => true,
        'question11_score' => true,
        'question12_score' => true,
        'question13_score' => true,
        'question14_score' => true,
        'question15_score' => true,
        'question16_score' => true,
        'question17_score' => true,
        'question18_score' => true,
        'question19_score' => true,
        'question20_score' => true,
        'total_score' => true,
        'created_at' => true,
        'modified_at' => true,
    ];

    /**
     * Convert Status label
     *
     * @return string
     */
    protected function _getStatusLabel(): string
    {
        switch($this->status) {
            case APPLICATION_STATUS_APPROVED:
                return '承認済み';
                break;

            case APPLICATION_STATUS_REJECTED:
                return '否認済み';
                break;

            default:
                return '認証待ち';
                break;
        }
    }
}
