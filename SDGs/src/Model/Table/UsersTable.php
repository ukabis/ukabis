<?php
declare(strict_types=1);

namespace App\Model\Table;

use Cake\Auth\DefaultPasswordHasher;
use Cake\ORM\Query;
use Cake\ORM\RulesChecker;
use Cake\ORM\Table;
use Cake\Validation\Validator;

/**
 * Users Model
 *
 * @method \App\Model\Entity\User newEmptyEntity()
 * @method \App\Model\Entity\User newEntity(array $data, array $options = [])
 * @method \App\Model\Entity\User[] newEntities(array $data, array $options = [])
 * @method \App\Model\Entity\User get($primaryKey, $options = [])
 * @method \App\Model\Entity\User findOrCreate($search, ?callable $callback = null, $options = [])
 * @method \App\Model\Entity\User patchEntity(\Cake\Datasource\EntityInterface $entity, array $data, array $options = [])
 * @method \App\Model\Entity\User[] patchEntities(iterable $entities, array $data, array $options = [])
 * @method \App\Model\Entity\User|false save(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\User saveOrFail(\Cake\Datasource\EntityInterface $entity, $options = [])
 * @method \App\Model\Entity\User[]|\Cake\Datasource\ResultSetInterface|false saveMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\User[]|\Cake\Datasource\ResultSetInterface saveManyOrFail(iterable $entities, $options = [])
 * @method \App\Model\Entity\User[]|\Cake\Datasource\ResultSetInterface|false deleteMany(iterable $entities, $options = [])
 * @method \App\Model\Entity\User[]|\Cake\Datasource\ResultSetInterface deleteManyOrFail(iterable $entities, $options = [])
 *
 * @mixin \Cake\ORM\Behavior\TimestampBehavior
 */
class UsersTable extends Table
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

        $this->setTable('users');
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
            ->email('email', false, INVALID_EMAIL_MESSAGE)
            ->maxLength('password', 255)
            ->requirePresence('email', 'create')
            ->notEmptyString('email', NOT_EMPTY_MESSAGE);

        $validator
            ->requirePresence('email_confirm', 'create', NOT_EMPTY_MESSAGE)
            ->notEmptyString('email_confirm', NOT_EMPTY_MESSAGE)
            ->add(
                'email_confirm',
                'custom',
                [
                    'rule' => function ($value, $context) {
                            if (isset($context['data']['email']) && trim($value) == trim($context['data']['email'])) {
                                return true;
                            }
                            return false;
                        },
                    'message' => EMAIL_CONFIRMATION_DOES_NOT_MATCH
                ]
            );

        $validator
            ->requirePresence('name', 'create')
            ->maxLength('password', 255)
            ->notEmptyString('name', NOT_EMPTY_MESSAGE);

        $validator
            ->scalar('password')
            ->maxLength('password', 255)
            ->requirePresence('password', 'create', NOT_EMPTY_MESSAGE)
            ->notEmptyString('password', NOT_EMPTY_MESSAGE)
            ->add(
                'password',
                'custom',
                [
                    'rule' => function ($value, $context) {
                        if (preg_match(REGEX_PASSWORD, $value)) {
                            return true;
                        }
                            return false;
                    },
                    'message' => INVALID_PASSWORD_FORMAT
                ]
            );

        $validator
            ->requirePresence('password_confirm', 'create', NOT_EMPTY_MESSAGE)
            ->notEmptyString('password_confirm', NOT_EMPTY_MESSAGE)
            ->add(
                'password_confirm',
                'custom',
                [
                    'rule' => function ($value, $context) {
                            if (isset($context['data']['password']) && $value == $context['data']['password']) {
                                return true;
                            }
                            return false;
                        },
                    'message' => PASSWORD_CONFIRMATION_DOES_NOT_MATCH
                ]
            );

        return $validator;
    }

    /**
     * Checker validation rules.
     *
     * @param \Cake\Validation\Validator $validator Validator instance.
     * @return \Cake\Validation\Validator
     */
    public function validationChecker(Validator $validator): Validator
    {
        $validator
            ->email('email', false, INVALID_EMAIL_MESSAGE)
            ->maxLength('password', 255)
            ->requirePresence('email', 'create')
            ->notEmptyString('email', NOT_EMPTY_MESSAGE);

        $validator
            ->requirePresence('name', 'create')
            ->maxLength('password', 255)
            ->notEmptyString('name', NOT_EMPTY_MESSAGE);

        $validator
            ->scalar('password')
            ->maxLength('password', 255)
            ->requirePresence('password', 'create', NOT_EMPTY_MESSAGE)
            ->notEmptyString('password', NOT_EMPTY_MESSAGE)
            ->add(
                'password',
                'custom',
                [
                    'rule' => function ($value, $context) {
                        if (preg_match(REGEX_PASSWORD, $value)) {
                            return true;
                        }
                            return false;
                    },
                    'message' => INVALID_PASSWORD_FORMAT
                ]
            );

        $validator
            ->requirePresence('password_confirm', 'create', NOT_EMPTY_MESSAGE)
            ->notEmptyString('password_confirm', NOT_EMPTY_MESSAGE)
            ->add(
                'password_confirm',
                'custom',
                [
                    'rule' => function ($value, $context) {
                            if (isset($context['data']['password']) && $value == $context['data']['password']) {
                                return true;
                            }
                            return false;
                        },
                    'message' => PASSWORD_CONFIRMATION_DOES_NOT_MATCH
                ]
            );

        $validator
            ->requirePresence('current_password', 'update', NOT_EMPTY_MESSAGE)
            ->notEmptyString('current_password', NOT_EMPTY_MESSAGE)
            ->add(
                'current_password',
                'current_password',
                [
                    'rule' => function($value, $context) {
                        $user = $this->findById($context['data']['id'])->first();

                        return(new DefaultPasswordHasher())->check($value, $user->password);
                    },
                    'message' => CURRENT_PASSWORD_INVALID
                ]
            );

        $validator
            ->allowEmptyString('new_password')
            ->scalar('new_password')
            ->maxLength('new_password', 255)
            ->add(
                'new_password',
                'custom',
                [
                    'rule' => function ($value, $context) {
                        if (preg_match(REGEX_PASSWORD, $value)) {
                            return true;
                        }
                            return false;
                    },
                    'message' => INVALID_PASSWORD_FORMAT
                ]
            );

        $validator
            ->add(
                'new_password_confirm',
                'custom',
                [
                    'rule' => function ($value, $context) {
                            if (isset($context['data']['new_password']) && $value == $context['data']['new_password']) {
                                return true;
                            }
                            return false;
                        },
                    'message' => PASSWORD_CONFIRMATION_DOES_NOT_MATCH
                ]
            );

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
        $rules->add($rules->isUnique(['email'], EMAIL_EXISTED_MESSAGE), ['errorField' => 'email']);

        return $rules;
    }
}
