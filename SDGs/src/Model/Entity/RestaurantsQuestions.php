<?php
declare(strict_types=1);

namespace App\Model\Entity;

use Cake\ORM\Entity;

/**
 * Question Entity
 *
 * @property int $id
 * @property int $office_id
 * @property object|string|null $answer_questions
 * @property int|null $status
 * @property int|null $question_1_menu_1_score
 * @property int|null $question_1_menu_2_score
 * @property int|null $question_1_menu_3_score
 * @property int|null $question_1_menu_4_score
 * @property int|null $question_1_menu_5_score
 * @property int|null $question_1_menu_6_score
 * @property int|null $question_1_menu_7_score
 * @property int|null $question_1_menu_8_score
 * @property int|null $question_1_menu_9_score
 * @property int|null $question_1_menu_10_score
 * @property int|null $question_2_score
 * @property int|null $question_3_score
 * @property int|null $question_4_score
 * @property int|null $question_5_score
 * @property int|null $question_6_score
 * @property int|null $question_7_menu_1_score
 * @property int|null $question_7_menu_2_score
 * @property int|null $question_7_menu_3_score
 * @property int|null $question_7_menu_4_score
 * @property int|null $question_7_menu_5_score
 * @property int|null $question_8_score
 * @property int|null $total_score
 * @property \Cake\I18n\FrozenTime $created_at
 * @property \Cake\I18n\FrozenTime $modified_at
 */
class RestaurantsQuestions extends Entity
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
        'question_1_menu_1_score' => true,
        'question_1_menu_2_score' => true,
        'question_1_menu_3_score' => true,
        'question_1_menu_4_score' => true,
        'question_1_menu_5_score' => true,
        'question_1_menu_6_score' => true,
        'question_1_menu_7_score' => true,
        'question_1_menu_8_score' => true,
        'question_1_menu_9_score' => true,
        'question_1_menu_10_score' => true,
        'question_2_score' => true,
        'question_3_score' => true,
        'question_4_score' => true,
        'question_5_score' => true,
        'question_6_score' => true,
        'question_7_menu_1_score' => true,
        'question_7_menu_2_score' => true,
        'question_7_menu_3_score' => true,
        'question_7_menu_4_score' => true,
        'question_7_menu_5_score' => true,
        'question_8_score' => true,
        'total_score' => true,
        'created_at' => true,
        'modified_at' => true,
    ];

    /**
     * Get Status label
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
