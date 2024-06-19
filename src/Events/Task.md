### База запланованих заходів.
- [ ] Заходи мають дату, час, тривалість, місце проведення, короткий опис, категорію.
- [ ] Можливість їх створення, читання, редагування та видалення.
- [ ] Дані зберігаються у файлі.
- [ ] Перегляд і показ списків справ на завтра, післязавтра і на певну, визначену користувачем, дату.
- [ ] Категоризація справ та їх пошук за фільтрами.
- [ ] видалення вчорашніх справ або перенесення на майбутнє.

- [ ] Аналіз «накладок» (перетинань планованих справ) та їх врахування при створенні нових заходів.
- [ ] Автоматичне нагадування про найближчі справи: за поточною датою і часом;
- [ ] Запит статистики та її експорт до файл

Events intersection:

Do you want to create a new event that overlaps with an existing event?

- [ ] Yes
- [ ] No, adjust the timing of the new event.

1.	Total number of events: Display the total count of all the planned events in the database.
2.	Event distribution by category: Show the distribution of events across different categories. 
This can help users understand the distribution of their activities and prioritize accordingly.
3.	Event distribution by date: Provide a breakdown of events based on dates. 
This can help users identify busy or free days and plan their schedule accordingly.
4.	Event duration statistics: Show statistics related to the duration of events, such as the average duration, the longest event, and the shortest event. 
This can help users understand the time commitment required for different activities.
5.	Overlapping events: Identify any overlapping events and display them as a statistic. 
This can help users avoid scheduling conflicts and ensure efficient use of their time.
6.	Event completion rate: Calculate the percentage of completed events out of the total planned events. 
This can provide users with insights into their productivity and task management.
7.	Event frequency: Show the frequency of events over a specific time period, such as daily, weekly, or monthly. 
This can help users identify patterns in their activities.

      To save edits of DataGrid row items with custom logic, you can handle the `RowEditEnding` event. This event is triggered when the user finishes editing a row, but before the changes are committed. You can add your custom logic in this event handler.

Here is a step-by-step guide:

1. Define the `RowEditEnding` event handler in your DataGrid. This can be done in the XAML file where your DataGrid is defined.

```xml
<DataGrid Name="myDataGrid" AutoGenerateColumns="False" RowEditEnding="MyDataGrid_RowEditEnding">
    <!-- Define your columns here -->
</DataGrid>
```

2. Implement the event handler in your code-behind file. In this method, you can access the edited row and its data. You can then apply your custom logic to decide whether to commit the changes or revert them.

```csharp
private void MyDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
{
    // Get the edited item
    var editedItem = e.Row.Item;

    // Apply your custom logic here
    // If the changes should not be committed based on your logic, you can cancel the event
    if (/* your condition */)
    {
        e.Cancel = true;
    }
    else
    {
        // If the changes should be committed, you can do so here
        // This might involve saving the changes to a database, for example
    }
}
```

Remember to replace `/* your condition */` with your actual condition. This condition will determine whether the changes made to the row should be committed or not.

Please note that this is a simple example. Depending on your application's requirements, you might need to implement more complex logic, handle exceptions, etc.