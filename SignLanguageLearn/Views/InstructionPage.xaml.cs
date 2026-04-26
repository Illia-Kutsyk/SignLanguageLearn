using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SignLanguageLearn.Views
{
    /// <summary>
    /// Логіка взаємодії для сторінки інструкцій (InstructionPage.xaml).
    /// Містить довідкову інформацію про те, як користуватися застосунком, 
    /// та елементи керування для повернення до попереднього розділу.
    /// </summary>
    public partial class InstructionPage : Page
    {
        /// <summary>
        /// Ініціалізує новий екземпляр класу <see cref="InstructionPage"/>.
        /// </summary>
        public InstructionPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Обробник натискання кнопки "Назад". 
        /// Використовує <see cref="NavigationService"/> для повернення користувача 
        /// на попередню сторінку в історії навігації.
        /// </summary>
        /// <param name="sender">Об'єкт, що викликав подію (кнопка).</param>
        /// <param name="e">Аргументи події.</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            // Перевіряємо, чи є в журналі навігації сторінка, на яку можна повернутися
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}