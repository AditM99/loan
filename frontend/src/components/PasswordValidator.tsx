import React from 'react';

interface PasswordValidatorProps {
  password: string;
}

interface ValidationRule {
  id: string;
  label: string;
  validator: (password: string) => boolean;
}

const validationRules: ValidationRule[] = [
  {
    id: 'length',
    label: 'At least 8 characters',
    validator: (password) => password.length >= 8
  },
  {
    id: 'letter',
    label: 'Contains letters',
    validator: (password) => /[a-zA-Z]/.test(password)
  },
  {
    id: 'number',
    label: 'Contains numbers',
    validator: (password) => /\d/.test(password)
  },
  {
    id: 'special',
    label: 'Contains special character',
    validator: (password) => /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)
  }
];

export function isPasswordValid(password: string){
  return validationRules.every(r => r.validator(password));
}

export function PasswordValidator({ password }: PasswordValidatorProps) {
  if (!password) return null;

  return (
    <div className="password-validator" style={{ marginTop: '8px', fontSize: '14px' }}>
      <div style={{ color: '#666', marginBottom: '4px' }}>Password requirements:</div>
      {validationRules.map((rule) => {
        const isValid = rule.validator(password);
        return (
          <div 
            key={rule.id} 
            style={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: '8px',
              color: isValid ? '#22c55e' : '#6b7280',
              marginBottom: '2px'
            }}
          >
            <span style={{ fontSize: '16px' }}>
              {isValid ? '✓' : '○'}
            </span>
            <span>{rule.label}</span>
          </div>
        );
      })}
    </div>
  );
}
