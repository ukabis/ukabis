<?php
declare(strict_types=1);
namespace App\Model\Entity;

use Cake\ORM\Entity;

/**
 * Office Entity
 *
 * @property int $id
 * @property string $email
 * @property string $password
 * @property int $office_type
 * @property string $offices_certified_rank
 * @property \Cake\I18n\FrozenTime|null $created
 * @property \Cake\I18n\FrozenTime|null $modified
 */
class Office extends Entity
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
        'business_id' => true,
        'office_type' => true,
        'name' => true,
        'name_kana' => true,
        'zip_code' =>true,
        'prefecture' => true,
        'city' => true,
        'street' => true,
        'tel' => true,
        'fax' => true,
        'offices_introduction' => true,
        'offices_pr_message' => false,
        'offices_certified_rank' => true,
        'offices_certified_file_path' => true,
        'profile_image_1' => true,
        'profile_image_2' => true,
        'created_at' => true,
        'modified_at' => true,
    ];

    /**
     *
     * @return array
     */
    protected function _getCertificate(): array
    {
        $certificateInfo = [
            'mark_image'      => '',
            'rank_label'      => 'なし'
        ];

        if ($this->office_type === OFFICE_RESTAURANT) {
            $certificateInfo =  $this->_getRestaurantCertificate();
        }

        if ($this->office_type === OFFICE_PRODUCER) {
            $certificateInfo = $this->_getFarmCertificate();
        }

        return $certificateInfo;
    }

    /**
     *
     * @return array
     */
    private function _getRestaurantCertificate(): array
    {
        switch($this->offices_certified_rank) {
            case GOLD:
                return [
                    'mark_image'      => RESTAURANT_GOLD_MARK_IMAGE,
                    'rank_label'      => 'ゴールド'
                ];

            case SILVER:
                return [
                    'mark_image'      => RESTAURANT_SILVER_MARK_IMAGE,
                    'rank_label'      => 'シルバー'
                ];

            case BRONZE:
                return [
                    'mark_image'      => RESTAURANT_BRONZE_MARK_IMAGE,
                    'rank_label'      => 'ブロンズ'
                ];

            case DENIED:
            default:
                return [
                    'mark_image'      => '',
                    'rank_label'      => 'なし'
                ];
        }
    }

    /**
     *
     * @return array
     */
    private function _getFarmCertificate(): array
    {
        switch($this->offices_certified_rank) {
            case GOLD:
                return [
                    'mark_image'      => FARM_GOLD_MARK_IMAGE,
                    'rank_label'      => 'ゴールド'
                ];

            case SILVER:
                return [
                    'mark_image'      => FARM_SILVER_MARK_IMAGE,
                    'rank_label'      => 'シルバー'
                ];

            case BRONZE:
                return [
                    'mark_image'      => FARM_BRONZE_MARK_IMAGE,
                    'rank_label'      => 'ブロンズ'
                ];

            case DENIED:
            default:
                return [
                    'mark_image'      => '',
                    'rank_label'      => 'なし'
                ];
        }
    }
}
