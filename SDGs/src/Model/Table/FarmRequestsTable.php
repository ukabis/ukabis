<?php
declare(strict_types=1);

namespace App\Model\Table;

use Cake\ORM\Table;
use Cake\ORM\RulesChecker;
use Cake\Validation\Validator;

/**
 * FarmRequests Model
 *
 * @method \App\Model\Entity\FarmRequests newEmptyEntity()
 * @method \App\Model\Entity\FarmRequests newEntity(array $data, array $options = [])
 * @method \App\Model\Entity\FarmRequests[] newEntities(array $data, array $options = [])
 * @method \App\Model\Entity\FarmRequests get($primaryKey, $options = [])
 * @method \App\Model\Entity\FarmRequests findOrCreate($search, ?callable $callback = null, $options = [])
 * @method \App\Model\Entity\FarmRequests patchEntity(\Cake\Datasource\EntityInterface $entity, array $data, array $options = [])
 * @method \App\Model\Entity\FarmRequests[] patchEntities(iterable $entities, array $data, array $options = [])
 * @method \App\Model\Entity\FarmRequests|false save(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\FarmRequests saveOrFail(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\FarmRequests[]|\Cake\Datasource\ResultSetInterface|false saveMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\FarmRequests[]|\Cake\Datasource\ResultSetInterface saveManyOrFail(iterable $entities, $options = [])
 * @method \App\Model\Entity\FarmRequests[]|\Cake\Datasource\ResultSetInterface|false deleteMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\FarmRequests[]|\Cake\Datasource\ResultSetInterface deleteManyOrFail(iterable $entities, $options = [])
 *
 * @mixin \Cake\ORM\Behavior\TimestampBehavior
 */
class FarmRequestsTable extends Table
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

        $this->setTable('farm_requests');
        $this->setEntityClass('FarmRequests');
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
        $this->belongsTo('Offices');
    }

    /**
     * Default validation rules.
     *
     * @param \Cake\Validation\Validator $validator Validator instance.
     * @return \Cake\Validation\Validator
     */
    public function validationDefault(Validator $validator): Validator
    {
        $validator->notEmptyString('office_id');
        $validator
            ->add(
                'food',
                'require_if',
                [
                    'rule' => function ($value, $context) {
                        if (!$context['data']['comment']) {
                            return true;
                        }

                        if ($context['data']['comment'] && $value) {
                            return true;
                        }

                        return false;
                    },
                    'message' => NOT_EMPTY_MESSAGE
                ]
            )
            ->maxLength('food', 20);

        $validator
            ->add(
                'comment',
                'require_if',
                [
                    'rule' => function ($value, $context) {
                        if (!$context['data']['food']) {
                            return true;
                        }

                        if ($context['data']['food'] && $value) {
                            return true;
                        }

                        return false;
                    },
                    'message' => NOT_EMPTY_MESSAGE
                ]
            )
            ->maxLength('comment', 20);

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
        $rules->add($rules->isUnique(['office_id', 'food'], REQUEST_FOOD_EXISTED_MESSAGE), ['errorField' => 'food']);

        return $rules;
    }

    /**
     * Get list farm request by office id
     */
    public function getListRequestByOfficeId(Table $table, int $officeId, array $selectField = [])
    {
        return $table->find('all', ['order' => ['modified_at' => 'desc']])
            ->select($selectField)
            ->where(['office_id' => $officeId])
            ->all();
    }

    /**
     * Search farm request with restaurant request
     */
    public function searchByRestaurantRequest(Table $table, array $restaurantRequests)
    {
        $conditions = [];
        foreach ($restaurantRequests as $request) {
            array_push($conditions, ['food LIKE' => "%$request->food%"]);
        }
        $query =  $table->find('all', [
            'conditions' => [
                'OR' => $conditions
            ]
        ])
        ->select(['id', 'food', 'comment', 'created_at'])
        ->contain(
            [
                'Offices.Businesses' => [
                    'fields' => [
                        'Offices.name'
                    ]
                ]
            ])
        ->order('FarmRequests.created_at DESC');

        return $query;
    }

    /**
     * Get latest farm request by office id
     */
    public function getLatestRequestByOfficeId(Table $table, int $officeId, array $selectField = [])
    {
        return $table
            ->find()
            ->select($selectField)
            ->where(['office_id' => $officeId])
            ->order(['created_at' => 'desc'])
            ->first();
    }
}
