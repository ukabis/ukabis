<?php
declare(strict_types=1);

namespace App\Model\Table;

use Cake\ORM\Query;
use Cake\ORM\RulesChecker;
use Cake\ORM\Table;
use Cake\Validation\Validator;

/**
 * Businesses Model
 *
 * @method \App\Model\Entity\Business newEmptyEntity()
 * @method \App\Model\Entity\Business newEntity(array $data, array $options = [])
 * @method \App\Model\Entity\Business[] newEntities(array $data, array $options = [])
 * @method \App\Model\Entity\Business get($primaryKey, $options = [])
 * @method \App\Model\Entity\Business findOrCreate($search, ?callable $callback = null, $options = [])
 * @method \App\Model\Entity\Business patchEntity(\Cake\Datasource\EntityInterface $entity, array $data, array $options = [])
 * @method \App\Model\Entity\Business[] patchEntities(iterable $entities, array $data, array $options = [])
 * @method \App\Model\Entity\Business|false save(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\Business saveOrFail(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\Business[]|\Cake\Datasource\ResultSetInterface|false saveMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\Business[]|\Cake\Datasource\ResultSetInterface saveManyOrFail(iterable $entities, $options = [])
 * @method \App\Model\Entity\Business[]|\Cake\Datasource\ResultSetInterface|false deleteMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\Business[]|\Cake\Datasource\ResultSetInterface deleteManyOrFail(iterable $entities, $options = [])
 *
 * @mixin \Cake\ORM\Behavior\TimestampBehavior
 */
class BusinessesTable extends Table
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

        $this->setTable('businesses');
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
            ->maxLength('name', 255)
            ->notEmptyString('name', NOT_EMPTY_MESSAGE)
            ->add('name', [
                'name' => [
                    'rule' => ['minLength', 2],
                    'message' => MIN_CHARACTER_OF_NAME_MESSAGE,
                ]
            ]);
        $validator
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
            ->requirePresence('representative_name')
            ->maxLength('representative_name', 255)
            ->notEmptyString('representative_name', NOT_EMPTY_MESSAGE);
        $validator
            ->requirePresence('tel')
            ->maxLength('tel', 20)
            ->notEmptyString('tel', NOT_EMPTY_MESSAGE);
        $validator
            ->email('email', false, INVALID_EMAIL_MESSAGE)
            ->maxLength('email', 255)
            ->requirePresence('email')
            ->notEmptyString('email', NOT_EMPTY_MESSAGE);

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
}
