<?php
declare(strict_types=1);

namespace App\Model\Table;

use App\Model\Entity\FarmsQuestions;
use Cake\Core\Configure;
use Cake\ORM\Query;
use Cake\ORM\Table;
use Cake\Validation\Validator;

/**
 * Questions Model
 *
 * @method \App\Model\Entity\FarmsQuestions newEmptyEntity()
 * @method \App\Model\Entity\FarmsQuestions newEntity(array $data, array $options = [])
 * @method \App\Model\Entity\FarmsQuestions[] newEntities(array $data, array $options = [])
 * @method \App\Model\Entity\FarmsQuestions get($primaryKey, $options = [])
 * @method \App\Model\Entity\FarmsQuestions findOrCreate($search, ?callable $callback = null, $options = [])
 * @method \App\Model\Entity\FarmsQuestions patchEntity(\Cake\Datasource\EntityInterface $entity, array $data, array $options = [])
 * @method \App\Model\Entity\FarmsQuestions[] patchEntities(iterable $entities, array $data, array $options = [])
 * @method \App\Model\Entity\FarmsQuestions|false save(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\FarmsQuestions saveOrFail(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\FarmsQuestions[]|\Cake\Datasource\ResultSetInterface|false saveMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\FarmsQuestions[]|\Cake\Datasource\ResultSetInterface saveManyOrFail(iterable $entities, $options = [])
 * @method \App\Model\Entity\FarmsQuestions[]|\Cake\Datasource\ResultSetInterface|false deleteMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\FarmsQuestions[]|\Cake\Datasource\ResultSetInterface deleteManyOrFail(iterable $entities, $options = [])
 *
 * @mixin \Cake\ORM\Behavior\TimestampBehavior
 */
class FarmQuestionsTable extends Table
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

        $this->setEntityClass(FarmsQuestions::class);
        $this->setTable('farm_questions');
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
        Configure::load('farm_questions', 'default');
        $farmQuestions = Configure::read('questions');
        $farmQuestions = json_decode(json_encode($farmQuestions), FALSE);

        $validator
            ->integer('office_id')
            ->notEmptyString('office_id', NOT_EMPTY_MESSAGE);

        foreach($farmQuestions as $key => $question) {
            $validator
                ->allowEmptyString("question{$key}_score");

            foreach($question->options as $index => $option) {
                $validator->add("question{$key}_score", "score_{$index}", [
                    'rule' => function ($value, $context) use ($index, $option){
                        $value = json_decode($value, true);
                        $maxPoint = $option->score ?? 0;

                        if (isset($value[$index]) && (int) $value[$index] > $maxPoint) {
                            return __('この回答に対応する最高スコアは {0} です。再入力してください。', $maxPoint);
                        }

                        return true;
                    }
                ]);
            }
        }

        $validator
            ->nonNegativeInteger('total_score')
            ->allowEmptyString('total_score');

        return $validator;
    }

    /**
     * Find Answer by Office ID
     *
     * @param Table $query
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
     * Get answer with relationships
     *
     * @param Table $table
     * @param int $id
     * @param array $relations
     *
     * @return array|\Cake\Datasource\EntityInterface
     */
    public function getAnwserWithRelationships(Table $table, int $id, array $relations = [])
    {
        return $table
            ->findById($id)
            ->contain($relations)
            ->firstOrFail();
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
                    'Offices.Businesses' => [
                        'fields' => [
                            'Offices.name',
                            'Businesses.representative_name'
                        ]
                    ]
                ]
            );
    }
}
