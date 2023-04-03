<?php
declare(strict_types=1);

namespace App\Model\Table;

use Cake\ORM\Query;
use Cake\ORM\RulesChecker;
use Cake\ORM\Table;
use Cake\Validation\Validator;

/**
 * Offices Model
 *
 * @method \App\Model\Entity\Office newEmptyEntity()
 * @method \App\Model\Entity\Office newEntity(array $data, array $options = [])
 * @method \App\Model\Entity\Office[] newEntities(array $data, array $options = [])
 * @method \App\Model\Entity\Office get($primaryKey, $options = [])
 * @method \App\Model\Entity\Office findOrCreate($search, ?callable $callback = null, $options = [])
 * @method \App\Model\Entity\Office patchEntity(\Cake\Datasource\EntityInterface $entity, array $data, array $options = [])
 * @method \App\Model\Entity\Office[] patchEntities(iterable $entities, array $data, array $options = [])
 * @method \App\Model\Entity\Office|false save(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\Office saveOrFail(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\Office[]|\Cake\Datasource\ResultSetInterface|false saveMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\Office[]|\Cake\Datasource\ResultSetInterface saveManyOrFail(iterable $entities, $options = [])
 * @method \App\Model\Entity\Office[]|\Cake\Datasource\ResultSetInterface|false deleteMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\Office[]|\Cake\Datasource\ResultSetInterface deleteManyOrFail(iterable $entities, $options = [])
 *
 * @mixin \Cake\ORM\Behavior\TimestampBehavior
 */
class OfficesTable extends Table
{
    /**
     * Initialize method
     *
     * @param array $config The configuration for the Table.
     * @return void
     */
    public function initialize(array $config): void
    {
        parent::initialize($config);

        $this->setTable('offices');
        $this->setDisplayField('id');
        $this->setPrimaryKey('id');

        $this->addBehavior('Timestamp', [
            'events' => [
                'Model.beforeSave' => [
                    'created_at' => 'new',
                    'modified_at' => 'always',
                ],
            ]
        ]);
        $this->hasOne('Users');
        $this->hasMany('FarmRequests');
        $this->belongsTo('Businesses');
    }

    /**
     * Default validation rules.
     *
     * @param \Cake\Validation\Validator $validator Validator instance.
     * @return \Cake\Validation\Validator
     */
    public function validationDefault(Validator $validator): Validator
    {
        $validator
            ->requirePresence('name')
            ->notEmptyString('name', NOT_EMPTY_MESSAGE)
            ->maxLength('name', 255)
            ->add('name', [
                'name' => [
                    'rule' => ['minLength', 2],
                    'message' => MIN_CHARACTER_OF_NAME_MESSAGE,
                ]
            ]);
        $validator
            ->scalar('name_kana')
            ->requirePresence('name_kana')
            ->maxLength('name_kana', 255)
            ->notEmptyString('name_kana', NOT_EMPTY_MESSAGE)
            ->add(
                'name_kana',
                'custom',
                [
                    'rule' => function ($value, $context) {
                        if (preg_match('/^[\x{30A0}-\x{30FF}]+$/u', $value) || preg_match('/^[\x{FF5F}-\x{FF9F}]+$/u', $value)) {
                            return true;
                        }
                        return false;
                    },
                    'message' => INPUT_ONLY_KANA
                ]
            );
        $validator
            ->requirePresence('zip_code')
            ->maxLength('zip_code', 20)
            ->notEmptyString('zip_code', NOT_EMPTY_MESSAGE);
        $validator
            ->requirePresence('prefecture')
            ->maxLength('prefecture', 255)
            ->notEmptyString('prefecture', NOT_EMPTY_MESSAGE);
        $validator
            ->requirePresence('city')
            ->maxLength('city', 255)
            ->notEmptyString('city', NOT_EMPTY_MESSAGE);
        $validator
            ->requirePresence('street')
            ->maxLength('street', 255)
            ->notEmptyString('street', NOT_EMPTY_MESSAGE);
        $validator
            ->requirePresence('tel')
            ->maxLength('tel', 20)
            ->notEmptyString('tel', NOT_EMPTY_MESSAGE);
        $validator
            ->requirePresence('offices_introduction')
            ->maxLength('offices_introduction', 1000)
            ->notEmptyString('offices_introduction', NOT_EMPTY_MESSAGE);
        $validator
            ->allowEmptyString('offices_certified_rank')
            ->add('offices_certified_rank', 'validValue', [
                'rule' => function($value, $context) {
                    return in_array($value, [GOLD, SILVER, BRONZE, DENIED, PENDING_VERIFICATION]);
                },
                'message' => NOT_EMPTY_RANK
            ]);
        $validator
            ->maxLength('offices_certified_file_path', 255)
            ->allowEmptyString('offices_certified_file_path');
        $validator
            ->allowEmptyFile('file_upload_1')
            ->add('file_upload_1', [
                'mimeType' => [
                    'rule' => ['mimeType', TYPES_ALLOW_UPLOAD],
                    'message' => implode(', ', TYPES_ALLOW_UPLOAD) . '形式のファイルを使用してください。',
                ],
                'fileSize' => [
                    'rule' => ['fileSize','<', MAX_UPLOAD_FILESIZE],
                    'message' => 'ファイルサイズは' . MAX_UPLOAD_FILESIZE. '以下のものを使用してください。',
                ]
            ]);
        $validator
            ->allowEmptyFile('file_upload_2')
            ->add('file_upload_2', [
                'mimeType' => [
                    'rule' => ['mimeType', TYPES_ALLOW_UPLOAD],
                    'message' => implode(', ', TYPES_ALLOW_UPLOAD) . '形式のファイルを使用してください。',
                ],
                'fileSize' => [
                    'rule' => ['fileSize','<', MAX_UPLOAD_FILESIZE],
                    'message' => 'ファイルサイズは' . MAX_UPLOAD_FILESIZE. '以下のものを使用してください。',
                ]
            ]);

        return $validator;
    }

    /**
     * Returns a rules checker object that will be used for validating
     * application integrity.
     *
     * @param \Cake\ORM\RulesChecker $rules The rules object to be modified.
     * @return \Cake\ORM\RulesChecker
     */
    public function buildRules(RulesChecker $rules): RulesChecker
    {
        $rules->add($rules->isUnique(['email']), ['errorField' => 'email']);

        return $rules;
    }

    /**
     * Search farm by condition
     *
     * @param Table $table
     * @param array $params = []
     *
     * @return \Cake\ORM\Query
     */
    public function searchFarmByCondition(Table $table, array $params = []): Query
    {
        $params['area'] = $params['area'] ?? $params['amp;area'] ?? null;
        $params['keyword'] = $params['keyword'] ?? $params['amp;keyword'] ?? null;
        $where = [];
        if (!empty($params['area']) && trim($params['area'])) {
            $params['area'] = trim($params['area']);
            array_push($where, ['prefecture LIKE' => '%' . $params['area'] . '%']);
            array_push($where, ['city LIKE' => '%' . $params['area'] . '%']);
            array_push($where, ['street LIKE' => '%' . $params['area'] . '%']);
        }
        $lastFarmRequestCondition = '(SELECT id FROM farm_requests WHERE office_id = Offices.id ORDER BY created_at DESC LIMIT 1)';
        if (!empty($params['keyword']) && trim($params['keyword'])) {
            $params['keyword'] = trim($params['keyword']);
            array_push($where, ['name LIKE' => '%' . $params['keyword'] . '%']);
            array_push($where, ['offices_introduction LIKE' => '%' . $params['keyword'] . '%']);
            array_push($where, ['LastFarmRequest.food LIKE' => '%' . $params['keyword'] . '%']);
            $lastFarmRequestCondition = "(SELECT id FROM farm_requests WHERE office_id = Offices.id AND farm_requests.food LIKE '%" . $params['keyword'] . "%' ORDER BY created_at DESC LIMIT 1)";
        }
        $conditions = [
            'office_type' => OFFICE_PRODUCER,
            'OR' => $where
        ];

        return $table->find('all', [
            'fields' => [
                'Offices.id',
                'Offices.name',
                'Offices.city',
                'Offices.office_type',
                'Offices.offices_introduction',
                'Offices.offices_certified_rank',
                'Offices.created_at',
                'LastFarmRequest.id',
                'LastFarmRequest.office_id',
                'LastFarmRequest.food',
                'LastFarmRequest.created_at'
            ],
            'conditions' => $conditions,
            'join' => [
                [
                    'table' => 'farm_requests',
                    'alias' => 'LastFarmRequest',
                    'type'  => 'left',
                    'conditions' => [
                        "LastFarmRequest.id = $lastFarmRequestCondition"
                    ],
                    'order' => ['LastFarmRequest.created_at' => 'desc']
                ]
            ]
        ]);
    }

    /**
     * Search restaurant by condition
     *
     * @param Table $table
     * @param array $params = []
     *
     * @return \Cake\ORM\Query
     */
    public function searchRestaurantByCondition(Table $table, array $params = []): Query
    {
        $params['area'] = $params['area'] ?? $params['amp;area'] ?? null;
        $params['keyword'] = $params['keyword'] ?? $params['amp;keyword'] ?? null;
        $where = [];
        if (!empty($params['area']) && trim($params['area'])) {
            $params['area'] = trim($params['area']);
            array_push($where, ['prefecture LIKE' => '%' . $params['area'] . '%']);
            array_push($where, ['city LIKE' => '%' . $params['area'] . '%']);
            array_push($where, ['street LIKE' => '%' . $params['area'] . '%']);
        }
        $lastRestaurantRequestCondition = '(SELECT id FROM restaurant_requests WHERE office_id = Offices.id ORDER BY created_at DESC LIMIT 1)';
        if (!empty($params['keyword']) && trim($params['keyword'])) {
            $params['keyword'] = trim($params['keyword']);
            array_push($where, ['name LIKE' => '%' . $params['keyword'] . '%']);
            array_push($where, ['offices_introduction LIKE' => '%' . $params['keyword'] . '%']);
            array_push($where, ['LastRestaurantRequest.food LIKE' => '%' . $params['keyword'] . '%']);
            $lastRestaurantRequestCondition = "(SELECT id FROM restaurant_requests WHERE office_id = Offices.id AND restaurant_requests.food LIKE '%" . $params['keyword'] . "%' ORDER BY created_at DESC LIMIT 1)";
        }
        $conditions = [
            'office_type' => OFFICE_RESTAURANT,
            'OR' => $where
        ];

        return $table->find('all', [
            'fields' => [
                'Offices.id',
                'Offices.name',
                'Offices.city',
                'Offices.office_type',
                'Offices.offices_introduction',
                'Offices.offices_certified_rank',
                'Offices.created_at',
                'LastRestaurantRequest.id',
                'LastRestaurantRequest.office_id',
                'LastRestaurantRequest.food',
                'LastRestaurantRequest.created_at'
            ],
            'conditions' => $conditions,
            'join' => [
                [
                    'table' => 'restaurant_requests',
                    'alias' => 'LastRestaurantRequest',
                    'type'  => 'left',
                    'conditions' => [
                        "LastRestaurantRequest.id = $lastRestaurantRequestCondition"
                    ],
                    'order' => ['LastRestaurantRequest.created_at' => 'desc']
                ]
            ]
        ]);
    }

    /**
     * Search office by condition
     *
     * @param Table $table
     * @param int $officeType
     * @param array $params = []
     *
     * @return \Cake\ORM\Query
     */
    public function searchOfficeByCondition(Table $table, int $officeType, array $params = []): Query
    {
        $params['area'] = $params['area'] ?? $params['amp;area'] ?? null;
        $params['keyword'] = $params['keyword'] ?? $params['amp;keyword'] ?? null;
        $where = [];
        if (!empty($params['area']) && trim($params['area'])) {
            $params['area'] = trim($params['area']);
            array_push($where, ['prefecture LIKE' => '%' . $params['area'] . '%']);
            array_push($where, ['city LIKE' => '%' . $params['area'] . '%']);
            array_push($where, ['street LIKE' => '%' . $params['area'] . '%']);
        }
        if (!empty($params['keyword']) && trim($params['keyword'])) {
            $params['keyword'] = trim($params['keyword']);
            array_push($where, ['name LIKE' => '%' . $params['keyword'] . '%']);
            array_push($where, ['offices_introduction LIKE' => '%' . $params['keyword'] . '%']);
        }
        $conditions = [
            'office_type' => $officeType,
            'offices_certified_rank IN' => [GOLD, SILVER, BRONZE],
            'OR' => $where
        ];
        
        if ($officeType == OFFICE_PRODUCER) {
            $lastFarmRequestCondition = '(SELECT id FROM farm_requests WHERE office_id = Offices.id ORDER BY created_at DESC LIMIT 1)';

            return $table->find('all', [
                'fields' => [
                    'Offices.id',
                    'Offices.name',
                    'Offices.city',
                    'Offices.office_type',
                    'Offices.offices_certified_rank',
                    'Offices.created_at',
                    'LastFarmRequest.id',
                    'LastFarmRequest.office_id',
                    'LastFarmRequest.food',
                    'LastFarmRequest.created_at'
                ],
                'conditions' => $conditions,
                'join' => [
                    [
                        'table' => 'farm_requests',
                        'alias' => 'LastFarmRequest',
                        'type'  => 'left',
                        'conditions' => [
                            "LastFarmRequest.id = $lastFarmRequestCondition"
                        ],
                        'order' => ['LastFarmRequest.created_at' => 'desc']
                    ]
                ]
            ]);
            
        }

        return $table->find('all', [
            'fields' => [
                'Offices.id',
                'Offices.name',
                'Offices.city',
                'Offices.office_type',
                'Offices.offices_certified_rank',
                'Offices.created_at'
            ],
            'conditions' => $conditions
        ]);
    }
}
