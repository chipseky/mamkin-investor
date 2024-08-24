import {Component} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {combineLatest, startWith} from "rxjs";
import {MatCard} from "@angular/material/card";
import {MatFormField, MatLabel} from "@angular/material/form-field";
import {MatInput} from "@angular/material/input";

@Component({
  selector: 'app-calculator',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatLabel,
    MatFormField,
    MatInput
  ],
  templateUrl: './calculator.component.html',
  styleUrl: './calculator.component.scss'
})
export class CalculatorComponent {
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    // Создаем форму с двумя полями для ввода чисел и третьим полем для суммы
    this.form = this.fb.group({
      buy_price: [100, Validators.required],
      buy_commission: [.001, Validators.required],
      sell_price: [101, Validators.required],
      sell_commission: [.001, Validators.required],
      profit: [{ value: '', disabled: true }]
    });

    combineLatest([
      this.form.get('buy_price')!.valueChanges.pipe(startWith(this.form.get('buy_price')!.value)),
      this.form.get('buy_commission')!.valueChanges.pipe(startWith(this.form.get('buy_commission')!.value)),
      this.form.get('sell_price')!.valueChanges.pipe(startWith(this.form.get('sell_price')!.value)),
      this.form.get('sell_commission')!.valueChanges.pipe(startWith(this.form.get('sell_commission')!.value))
    ]).subscribe(([buyPrice, buyCommission, sellPrice, sellCommission]) => {
      const profit =
        -Number(buyPrice) +
        -Number(buyPrice) * Number(buyCommission) +
        Number(sellPrice) +
        -Number(sellPrice) * Number(sellCommission);
      this.form.get('profit')!.setValue(profit, { emitEvent: false }); // Обновляем значение поля sum
    });
  }
}
