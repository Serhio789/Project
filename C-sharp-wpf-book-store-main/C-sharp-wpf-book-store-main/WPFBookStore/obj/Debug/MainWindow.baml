   M S B A M L   `   `   `  ����G  CWPFBookStore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:WPFBookStore.ControlsWPFBookStore.Controls  5   	   +clr-namespace:WPFBookStoreWPFBookStore  5   	       WPFBookStore.MainWindow   5   	   ,ctls#clr-namespace:WPFBookStore.Controls  5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:WPFBookStore     5   	   -   5        $
��home��5        � Title$  
MainWindow��5	   	   $	��650��6   $
��1000��6)    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowStyle$
 None=�6.   
������  � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	   $��Poppins)�6   .��5      ��    PrimaryBackgroundColor#���   6    �� CornerRadius$ 8��5      ._�5   
   �  � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   .+�5      �� $��0��6     SecundaryBackgroundColor#���  5      $ 8 0 0 8��5      ._�5      � -   5         �Name$ GridNav��5      $��60��6(    � RowDefinitions 5      �� 5      $��60 �6(   �� 5      $��* �6(   .+�5       � $��0��6   .+�5!      �� $��14 0 0 0q�5"      $��32��5#      $��24��6*   $
��Left=�5$      ��5%      �� 5&   "   	 �� ImageSource$	 Resources/BooKLogo.png��6-   
 j� Stretch$
 Uniform=�6R   5(      �� 5*      $��
Book Store��6      PrimaryTextColor#���  5+       $
��Bold"�5,       $��16(�5-       $��Center=�5.       $
��Left=�5/       $��60 0 0 0q�50       51      �� 54      $��1��6!   $��
-1 40 0 20q�6.   .+�55      �� -   5         ��Name$ btnHome��55   !     PopupButtonStyle#���  62   .��56      $   WPFBookStore.Controls.MenuItem     	GroupName$ MenuItem��6,     PrimaryBlueColor   IndicatorBrush# �  6A     home   Icon# �  6u      	IconWidth$ 16��6�      Text$
 Home��6�   $��Center=�6�   57      �� 58      -   5        $ btnDashboard��58   !   #���  67   .��59        $ MenuItem��6,   # �  6A     	dashboard# �  6u   $ 16��6�   $ View All Books��6�   $��Center=�6�   5:      �� 5;      -   5        $ btnProducts��5;   !   #���  66   .��5<        $   WPFBookStore.Controls.MenuItem   Loaded$ MenuItem_Loaded��6�   $ MenuItem��6,   # �  6A     car# �  6u   $ 16��6�   $ Add New Book��6�   $��Center=�6�   6�   5=      5>      � 5B      -   5         �Name$ Popup��5B       � AllowsTransparency .  � HorizontalOffset$ 0��6G    � VerticalOffset$ 10��6\   .��5C      %   WPFBookStore.Controls.UserPopup  -   5          Name$ Header��5C   )   #���  69   5D      5G      5H      � 5J      $��1��6    5K      �� 5L      $��60 �6$   �� 5M      $��* �6$   .+�5P      d� -   5         d�Name$ Tg_Btn��5P      $��1��6/   $��Center=�6?     iToggleButtonStyle#���  6Z   $
��Left=�6�   $��0q�6�   
������ $��
10,15,0,15q�6�   $��30��6�   $��30��6�    � Triggers 5Q      :� 5R       :� RoutedEvent$ ToggleButton.Unchecked��6'   .[�5S      �� .��5T   "   �� -	   5         ��Name$ HideStackPanel��5T   -   .��5U   &   W�  �� 
TargetName$ GridNav��66    �� TargetProperty$ Width�6V    g� 	BeginTime$ 0:0:0X�6x    W� From$	 210X�6�   	 W� To$ 60X�6�     g� Duration$  0:0:0.5D�6�   5V   '   5W   #   5X      5Y      :� 5Z      $ ToggleButton.Checked��6'   .[�5[      �� .��5\   "   �� -
   5        $ ShowStackPanel��5\   -   .��5]   &   W� $ GridNav��66   $ Width�6V   $ 0:0:0X�6x   $ 60X�6�   $	 210X�6�   $  0:0:0.5D�6�   5^   '   5_   #   5`      5a      5c      �� 5f      $��0��6   $��Right=�6*   $	��Top=�6F   $��0,5,10,0q�6^   $��30��6p   ! �� Orientation$! 
Horizontal=�6|   $��RightToLeft=�6�   $��2��6�   .+�5g      �� -   5        $ btnClose��5g       	 IconButtonsStyle#��[	 6/    	
 close#��� 
 6Y   
" � Tag$" IsCloseButton��6�   �� 5h      -   5        $ 
btnRestore��5h      $	��5,0q�61   ��5i      �� 5j      # �� 
TargetType"# ��6$   $ �� BasedOn#$ [	 6A   .�5k   "   �� % �� Property% � ��  maximize& �� Value#& �  6<   ' �� Triggers' 5l   "   �� 5m   &   ( �� Binding( 62   �� ) �� ElementName$
) home��* �� Path$* WindowState�+ �� Value+ 	Maximized6i   .a�5n   *   �� % � ��  restore#& �  6D   5o   '   5q      5s      �� 5t      -   5        $ btnMinimize��5t      #��� 	 62     minimize#���  6]   6�   5u       � 5x      -   5        $��
fContainer��5y      $��1��5x      $��20q�5z      ,  � NavigationUIVisibility$, Hidden=�5{      5}      5      5�      5�      5�      