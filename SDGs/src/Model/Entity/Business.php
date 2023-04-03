<?php
declare(strict_types=1);
namespace App\Model\Entity;

use Cake\ORM\Entity;

/**
 * Business Entity
 *
 * @property int $id
 * @property string $email
 * @property string $password
 * @property \Cake\I18n\FrozenTime|null $created
 * @property \Cake\I18n\FrozenTime|null $modified
 */
class Business extends Entity
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
        'name' => true,
        'name_kana' => true,
        'email' => true,
        'zip_code' =>true,
        'prefecture' => true,
        'city' => true,
        'street' => true,
        'representative_name' => true,
        'tel' => true,
        'fax' => true,
        'homepage_url' => true,
        'created_at' => true,
        'modified_at' => true,
    ];
}
