<?php
declare(strict_types=1);

namespace App\Model\Table;

use Cake\ORM\Query;
use Cake\ORM\Table;
use Cake\Validation\Validator;
use App\Model\Entity\RestaurantsQuestions;

/**
 * Questions Model
 *
 * @method \App\Model\Entity\RestaurantsQuestions newEmptyEntity()
 * @method \App\Model\Entity\RestaurantsQuestions newEntity(array $data, array $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions[] newEntities(array $data, array $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions get($primaryKey, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions findOrCreate($search, ?callable $callback = null, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions patchEntity(\Cake\Datasource\EntityInterface $entity, array $data, array $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions[] patchEntities(iterable $entities, array $data, array $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions|false save(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions saveOrFail(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions[]|\Cake\Datasource\ResultSetInterface|false saveMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions[]|\Cake\Datasource\ResultSetInterface saveManyOrFail(iterable $entities, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions[]|\Cake\Datasource\ResultSetInterface|false deleteMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\RestaurantsQuestions[]|\Cake\Datasource\ResultSetInterface deleteManyOrFail(iterable $entities, $options = [])
 *
 * @mixin \Cake\ORM\Behavior\TimestampBehavior
 */
class RestaurantQuestionsTable extends Table
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
        $this->setEntityClass(RestaurantsQuestions::class);
        $this->setTable('restaurant_questions');
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
        $validator
            ->integer('office_id')
            ->notEmptyString('office_id', NOT_EMPTY_MESSAGE);

        $validator
            ->integer('status')
            ->allowEmptyString('status');


        for ($i = 1; $i <= 10; $i++) {
            $validator
                ->allowEmptyString("question_1_menu_{$i}_score")
                ->add("question_1_menu_{$i}_score", 'score', [
                    'rule' => (function($value, $context) {
                        return $value['score'] <= 2;
                    }),
                    'message' => 'この回答に対応する最高スコアは 2 です。再入力してください。'
                ]);
        }

        for ($i = 1; $i <= 5; $i++) {
            $validator
                ->allowEmptyString("question_7_menu_{$i}_score.score")
                ->add("question_7_menu_{$i}_score", 'score', [
                    'rule' => (function($value, $context) {
                        return $value['score'] <= 2;
                    }),
                    'message' => 'この回答に対応する最高スコアは 2 です。再入力してください。'
                ]);
        }

        $validator
            ->allowEmptyString('total_score');

        return $validator;
    }

    /**
     * Find Answer by Office ID
     *
     * @param Table $table
     * @param array $options
     * @return array|\Cake\Datasource\EntityInterface
     */
    public function getByOfficeId(Table $table, array $options)
    {
        $officeId = $options['office_id'];

        return $table
            ->find()
            ->where(['office_id' => $officeId])
            ->first();
    }

    /**
     * Get query list restaurant's answers
     *
     * @param Table $table
     * @param array $options = []
     *
     * @return \Cake\ORM\Query
     */
    public function getListAnswer(Table $table, array $options = []): Query
    {
        return $table
            ->find('all', $options)
            ->contain(
                [
                    'Offices' => function (Query $query) {
                        return $query
                            ->select('name')
                            ->contain(['Businesses' => function (Query $query) {
                                return $query->select(['representative_name']);
                            }]);
                    }
                ]
            );
    }
}
