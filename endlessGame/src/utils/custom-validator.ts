import { AbstractControl } from '@angular/forms';

export class CustomValidator{
  
  static numeric(control: AbstractControl) {
    let val = control.value;
    
    if (val === null || val === '' || val < 0) return null;
    
    if (!val.match(/^[0-9]+(\.?[0-9]+)?$/)) return { 'invalidNumber': true };
    
    return null;
  }
}